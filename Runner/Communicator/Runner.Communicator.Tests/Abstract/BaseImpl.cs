using Runner.Communicator.Abstract;
using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Abstract
{
    public class BaseImpl : SocketBase
    {
        public Func<CancellationToken, bool>? OnConnected;

        private bool _isConnected;
        private BytesReader? _data;
        private ManualResetEvent _manualResetEvent;

        public BaseImpl(int timeout)
            : base(timeout, new CancellationToken())
        {
            _manualResetEvent = new ManualResetEvent(false);
        }

        protected override bool IsConnected()
        {
            return _isConnected;
        }

        protected override Task DoConnectAsync(CancellationToken cancellationToken)
        {
            if (OnConnected != null)
            {
                _isConnected = OnConnected(cancellationToken);
            }
            return Task.CompletedTask;
        }

        protected override Task<byte[]> DoReadAsync(CancellationToken cancellationToken, uint length)
        {
            while (_data == null)
            {
                _manualResetEvent.WaitOne();
                _manualResetEvent.Reset();
            }
            if (length <= _data.Left)
            {
                var data = _data.ReadBytes(length);
                if (_data.Left == 0)
                {
                    _data = null;
                }
                return Task.FromResult(data);
            }
            throw new Exception("No Data!");
        }

        protected override Task DoSendAsync(CancellationToken cancellationToken, byte[] data)
        {
            var buffer = new byte[data.Length];
            Array.Copy(data, buffer, data.Length);
            _data = new BytesReader(buffer);
            _manualResetEvent.Set();
            return Task.CompletedTask;
        }

        protected override async Task<byte[]?> DoProcessRequest(byte[] data, MessagePort port)
        {
            return data;
        }
    }
}
