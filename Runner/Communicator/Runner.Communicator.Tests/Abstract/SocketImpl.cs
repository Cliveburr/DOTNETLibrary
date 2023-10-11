using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Abstract;
using Runner.Communicator.Model;

namespace Runner.Communicator.Tests
{
    public class SocketImpl : SocketTcp
    {
        private string _hostname;
        private int _port;
        private Func<Message, Task<Message>>? _process;

        public SocketImpl(TcpClient? tcpClient = null, Func<Message, Task<Message>>? process = null)
            : base(tcpClient)
        {
            _process = process;
            _hostname = "";
        }

        public Task ConnectAsync(string hostname, int port)
        {
            _hostname = hostname;
            _port = port;
            return ConnectAsync();
        }

        protected override async Task DoConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpClient?.Close();
            }
            catch { }
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_hostname, _port, CancellationToken);
        }

        private ManualResetEvent? _serverReady;

        public void Start(ManualResetEvent serverReady)
        {
            _serverReady = serverReady;
            Task.Run(StartAsync);
        }

        private async Task StartAsync()
        {
            _serverReady?.Set();
            while (!CancellationToken.IsCancellationRequested)
            {
                var message = await ReceiveMessage();

                var response = await _process!(message);
                await SendMessage(response);
            }
        }
    }
}
