using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;

namespace Runner.Communicator
{
    public class Server : IDisposable
    {
        public delegate void OnErrorDelegate(Server server, ServerConnection? connection, Exception err);
        public event OnErrorDelegate? OnError;
        public CancellationToken CancellationToken { get; private set; }
        public List<ServerConnection> Connections { get; private set; }
        public string? FileUploadDirectory { get; set; }

        private ushort _nextClientId;
        private TcpListener _listener;
        private ServerServices? _serverServices;
        private IServiceCollection _serviceCollection;

        public Server(int port, IServiceCollection services)
        {
            _serviceCollection = services;
            _listener = new TcpListener(IPAddress.Any, port);
            Connections = new List<ServerConnection>();
            _nextClientId = 1;
        }

        public void Dispose()
        {
            try
            {
                _listener.Stop();
                Connections.ForEach(c => c.Stop());
            }
            catch { }
        }

        public CancellationToken Start()
        {
            return Start(new CancellationToken());
        }

        public CancellationToken Start(CancellationToken cancellationToken)
        {
            _listener.Start();
            CancellationToken = cancellationToken;
            Task.Run(StartAsync);
            return CancellationToken;
        }

        public void Stop()
        {
            Dispose();
        }

        private ushort GetNextId()
        {
            if (_nextClientId == ushort.MaxValue)
            {
                _nextClientId = 1;
                return ushort.MaxValue;
            }
            else
            {
                return _nextClientId++;
            }
        }

        private async Task StartAsync()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                if (!_listener.Server.IsBound)
                {
                    throw new Exception("Lister not started!");
                }
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync(CancellationToken);
                    if (tcpClient != null)
                    {
                        var temp = new ServerConnection(tcpClient, this, CancellationToken);
                        var id = await temp.ShakeHand(GetNextId);

                        var connection = Connections
                            .FirstOrDefault(c => c.Id == id);
                        if (connection == null)
                        {
                            lock (Connections)
                            {
                                Connections.Add(temp);
                            }
                            temp.OnError += OnErrorConnection_Handler;
                            temp.OnClose += OnCloseConnection_Handler;
                            temp.Start();
                        }
                        else
                        {
                            connection.ReplaceTcpClient(tcpClient);
                        }
                    }
                }
                catch (Exception err)
                {
                    _ = Task.Run(() => OnError?.Invoke(this, null, err));
                }
            }
        }

        private void OnErrorConnection_Handler(ServerConnection sender, Exception err)
        {
            Task.Run(() => OnError?.Invoke(this, sender, err));
        }

        private void OnCloseConnection_Handler(ServerConnection sender)
        {
            lock (Connections)
            {
                Connections.Remove(sender);
            }
        }

        public ServerServices Services
        {
            get
            {
                if (_serverServices == null)
                {
                    _serverServices = new ServerServices(_serviceCollection);
                }
                return _serverServices;
            }
        }
    }
}