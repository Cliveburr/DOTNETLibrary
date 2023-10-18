using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;
using Runner.Communicator.Helpers;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;

namespace Runner.Communicator
{
    public class Server : IDisposable
    {
        public delegate void OnErrorDelegate(object sender, Exception err);
        public event OnErrorDelegate? OnError;

        public CancellationToken CancellationToken { get; private set; }
        public List<ServerConnection> Connections { get; private set; }
        //public string? FileUploadDirectory { get; set; }
        public int Timeout { get; set; }

        private ushort _nextClientId;
        private TcpListener _listener;
        private IServiceProvider _serviceProvider;

        public Server(int port, IServiceProvider serviceProvider, int timeout = 180000)
        {
            _serviceProvider = serviceProvider;
            _listener = new TcpListener(IPAddress.Any, port);
            Connections = new List<ServerConnection>();
            _nextClientId = 1;
            Timeout = timeout;
        }

        public void Dispose()
        {
            try
            {
                _listener.Stop();
                Connections.ForEach(c => c.Dispose());
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
                        var shakeHandResult = await ShakeHand(tcpClient);
                        if (shakeHandResult.Success)
                        {
                            var connection = Connections
                                .FirstOrDefault(c => c.Id == shakeHandResult.ClientId);
                            if (connection == null)
                            {
                                var serviceScope = _serviceProvider.CreateScope();
                                var serverConnection = new ServerConnection(tcpClient, shakeHandResult.ClientId, serviceScope, CancellationToken);
                                lock (Connections)
                                {
                                    Connections.Add(serverConnection);
                                }
                                serverConnection.OnError += OnErrorConnection_Handler;
                                serverConnection.OnClose += OnCloseConnection_Handler;
                                serverConnection.StartReceive();
                            }
                            else
                            {
                                connection.ReplaceTcpClient(tcpClient);
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    _ = Task.Run(() => OnError?.Invoke(this, err));
                }
            }
        }

        private async Task<(bool Success, ushort ClientId)> ShakeHand(TcpClient tcpClient)
        {
            ushort clientId;
            try
            {
                var readTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
                if (Timeout > 0)
                {
                    readTimeoutCancellation.CancelAfter(Timeout);
                }
                var requestLenght = 4; // (ushort = 2 byte) * 2
                var requestData = new byte[requestLenght];
                await tcpClient.GetStream().ReadExactlyAsync(requestData, 0, requestLenght, readTimeoutCancellation.Token);

                var reader = new BytesReader(requestData);
                clientId = reader.ReadUInt16();
                var ack = reader.ReadUInt16();
                if (ack != 1234)
                {
                    throw new Exception("ShakeHand ack wrong!");
                }
            }
            catch (Exception err)
            {
                try
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                }
                catch { }
                OnErrorConnection_Handler(this, err);
                return (false, 0);
            }

            if (clientId == 0)
            {
                clientId = GetNextId();
            }

            try
            {
                var writer = new BytesWriter();
                writer.WriteUInt16(clientId);
                writer.WriteUInt16(1234);

                var sendTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
                if (Timeout > 0)
                {
                    sendTimeoutCancellation.CancelAfter(Timeout);
                }
                await tcpClient.GetStream().WriteAsync(writer.GetBytes(), sendTimeoutCancellation.Token);

                return (true, clientId);
            }
            catch (Exception err)
            {
                try
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                }
                catch { }
                OnErrorConnection_Handler(this, err);
                return (false, 0);
            }
        }

        private void OnErrorConnection_Handler(object sender, Exception err)
        {
            Task.Run(() => OnError?.Invoke(sender, err));
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