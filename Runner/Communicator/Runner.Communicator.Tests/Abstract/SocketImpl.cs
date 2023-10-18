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
        private Func<byte[], MessagePort, Task<byte[]?>> _process;

        public SocketImpl(TcpClient? tcpClient = null, Func<byte[], MessagePort, Task<byte[]?>> process = null)
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

        protected override Task<byte[]?> DoProcessRequest(byte[] data, MessagePort port)
        {
            return _process(data, port);
        }
    }
}
