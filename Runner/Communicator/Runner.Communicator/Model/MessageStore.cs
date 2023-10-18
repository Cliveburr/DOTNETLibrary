using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Model
{
    internal class MessageStore
    {
        public CancellationToken CancellationToken { get => _cancellationTokenSource.Token; }
        public Message Message { get; set; }
        public bool WaitReponse { get; set; }

        private byte[]? _data;
        private Exception? _err;
        private ManualResetEvent _manualReset;
        private CancellationTokenSource _cancellationTokenSource;

        public MessageStore(Message message, CancellationTokenSource cancellationTokenSource)
        {
            Message = message;
            WaitReponse = false;
            _cancellationTokenSource = cancellationTokenSource;
            _manualReset = new ManualResetEvent(false);
        }

        public void Release(byte[]? data = null)
        {
            _data = data;
            _manualReset.Set();
        }

        public void Error(Exception err)
        {
            _err = err;
            _manualReset.Set();
        }

        public Task WaitAsync()
        {
            return Task.Run(() =>
            {
                WaitHandle.WaitAny(new[] { _cancellationTokenSource.Token.WaitHandle, _manualReset });
                if (_err != null)
                {
                    throw _err;
                }
            });
        }

        public Task<byte[]> WaitDataAsync()
        {
            return Task<byte[]>.Run(() =>
            {
                WaitHandle.WaitAny(new[] {
                    _cancellationTokenSource.Token.WaitHandle,
                    _manualReset
                });
                if (_err != null)
                {
                    throw _err;
                }
                else if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new TimeoutException("WaitDataAsync timeout");
                }
                else if (_data == null)
                {
                    throw new Exception("Invalid release without data!");
                }
                return _data;
            });
        }

        public void SetTimeout(Action<MessageStore, Exception?> timeoutEvent)
        {
            _ = Task.Run(() =>
            {
                WaitHandle.WaitAny(new[] {
                    _cancellationTokenSource.Token.WaitHandle,
                    _manualReset
                });
                if (_err != null)
                {
                    timeoutEvent(this, _err);
                }
                else if (_cancellationTokenSource.IsCancellationRequested)
                {
                    timeoutEvent(this, new TimeoutException("MessageStore timeout"));
                }
            });
        }
    }
}
