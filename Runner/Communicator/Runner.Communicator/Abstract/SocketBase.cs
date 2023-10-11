using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Runner.Communicator.Abstract
{
    public abstract class SocketBase : IDisposable
    {
        public int Timeout { get; set; }
        public CancellationToken CancellationToken { get => _cancellationTokenSource.Token; }

        protected CancellationTokenSource _cancellationTokenSource;
        protected abstract bool IsConnected();
        protected abstract Task DoConnectAsync(CancellationToken cancellationToken);
        protected abstract Task DoSendAsync(CancellationToken cancellationToken, byte[] data);
        protected abstract Task<byte[]> DoReadAsync(CancellationToken cancellationToken, uint length);

        private ControlAsync _singleCall;

        protected SocketBase(int timeout, CancellationToken cancellationToken)
        {
            Timeout = timeout;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _singleCall = new ControlAsync();
        }

        public virtual void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public Task ConnectAsync()
        {
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            return DoConnectAsync(timeoutCancellation.Token);
        }

        private async Task InnerSendMessage(Message message)
        {
            if (!IsConnected())
            {
                await ConnectAsync();
            }
            var data = message.Head
                .GetBytes()
                .Concat(message.Data)
                .ToArray();
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            await DoSendAsync(timeoutCancellation.Token, data);
        }

        private async Task<Message> InnerReceiveMessage()
        {
            if (!IsConnected())
            {
                await ConnectAsync();
            }
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var headData = await DoReadAsync(timeoutCancellation.Token, (uint)MessageHead.HeadLenght);
            var head = MessageHead.Parse(headData);
            var data = await DoReadAsync(timeoutCancellation.Token, head.Lenght);
            return new Message(head, data);
        }

        private async Task<Message?> SingleCallAtOnce(Message? message, int typeCall)
        {
            Message? response = null;
            await _singleCall.CheckToPass();

            switch (typeCall)
            {
                case 0:
                {
                    await InnerSendMessage(message!);
                    break;
                }
                case 1:
                {
                    await InnerSendMessage(message!);
                    response = await InnerReceiveMessage();
                    break;
                }
                case 2:
                {
                    response = await InnerReceiveMessage();
                    break;
                }
            }

            _singleCall.ReleasePass();
            return response;
        }

        public async Task SendMessage(Message message)
        {
            _ = await SingleCallAtOnce(message, 0);
        }

        public Task<Message> SendAndReceive(Message message)
        {
            return SingleCallAtOnce(message, 1)!;
        }

        public Task<Message> ReceiveMessage()
        {
            return SingleCallAtOnce(null, 2)!;
        }
    }
}
