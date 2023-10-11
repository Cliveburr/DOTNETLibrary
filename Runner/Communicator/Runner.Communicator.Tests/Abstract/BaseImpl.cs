using Runner.Communicator.Abstract;
using Runner.Communicator.Helpers;
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

        public BaseImpl(int timeout)
            : base(timeout, new CancellationToken())
        {
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
            if (_data != null)
            {
                if (length <= _data.Left)
                {
                    var data = _data.ReadBytes(length);
                    if (_data.Left == 0)
                    {
                        _data = null;
                    }
                    return Task.FromResult(data);
                }
            }
            throw new Exception("No Data!");
        }

        protected override Task DoSendAsync(CancellationToken cancellationToken, byte[] data)
        {
            var buffer = new byte[data.Length];
            Array.Copy(data, buffer, data.Length);
            _data = new BytesReader(buffer);
            return Task.CompletedTask;
        }
    }
}
