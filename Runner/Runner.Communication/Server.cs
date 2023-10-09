using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Runner.Communication.Model;

namespace Runner.Communication
{
    public class Server : IDisposable
    {
        public delegate void OnErrorDelegate(Server server, ServerConnection? connection, Exception err);
        public event OnErrorDelegate? OnError;
        public CancellationToken CancellationToken { get; private set; }
        public List<ServerServices> Services { get; private set; }
        public IServiceCollection ServiceCollection { get; private set; }
        public List<ServerConnection> Connections { get; private set; }
        public string? FileUploadDirectory { get; set; }

        private ushort _nextClientId;
        private TcpListener _listener;

        public Server(int port, IServiceCollection services)
        {
            ServiceCollection = services;
            _listener = new TcpListener(IPAddress.Any, port);
            Connections = new List<ServerConnection>();
            Services = new List<ServerServices>();
            _nextClientId = 1;
        }

        public Server Add<I, S>() 
            where I : class
            where S : class, I
        {
            ServiceCollection
                .AddScoped<I, S>();
            Services.Add(new ServerServices
            {
                Interface = typeof(I),
                Implementation = typeof(S)
            });
            return this;
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

        public ushort GetNextId()
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
                        var temp = new ServerConnection(tcpClient, this);
                        var id = await temp.ShakeHand();

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
    }
}