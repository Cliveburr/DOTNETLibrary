using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Runner.Communicator.Model;

namespace Runner.Communicator.Abstract
{
    public abstract class SocketBase2
    {
        public CancellationToken CancellationToken { get; set; }
        public int MaxAttempts { get; set; }
        public int Timeout { get; set; }

        protected abstract bool IsConnected();
        protected abstract Task DoSafeConnectAsync(ConnectContext ctx);
        protected abstract Task DoSafeSendAsync(SendReceiveContext ctx);
        protected abstract Task DoSafeReadAsync(SendReceiveContext ctx);

        protected SocketBase2(int maxAttempts, int timeout, CancellationToken cancellationToken)
        {
            MaxAttempts = maxAttempts;
            Timeout = timeout;
            CancellationToken = cancellationToken;
        }

        protected SocketBase2(int maxAttempts, int timeout)
            : this(maxAttempts, timeout, new CancellationToken())
        {
        }

        protected SocketBase2(int maxAttempts)
            : this(maxAttempts, 3000, new CancellationToken())
        {
        }

        protected SocketBase2()
            : this(3, 3000, new CancellationToken())
        {
        }

        protected class SendReceiveContext
        {
            public bool IsSuccess { get; set; }
            public bool IsDisconnected { get; set; }
            public required byte[] Data { get; set; }
            public int Attempts { get; set; }
            public DateTime TimeoutDateTime { get; set; }
            public CancellationToken CancellationToken { get; set; }
            public Exception? Exception { get; set; }
        }

        protected class ConnectContext
        {
            public bool IsConnected { get; set; }
            public int Attempts { get; set; }
            public DateTime TimeoutDateTime { get; set; }
            public CancellationToken CancellationToken { get; set; }
            public Exception? Exception { get; set; }
        }

        public async Task ConnectAsync()
        {
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var ctx = new ConnectContext
            {
                IsConnected = false,
                Attempts = 0,
                TimeoutDateTime = DateTime.Now.AddMilliseconds(Timeout),
                CancellationToken = timeoutCancellation.Token
            };
            await SafeConnectAsync(ctx);
            if (ctx.IsConnected)
            {
                return;
            }
            else
            {
                if (ctx.Exception != null)
                {
                    throw ctx.Exception;
                }
                else if (ctx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {ctx.Attempts}");
                }
                else if (DateTime.Now >= ctx.TimeoutDateTime)
                {
                    throw new TimeoutException($"Connection time: {DateTime.Now - ctx.TimeoutDateTime}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private async Task SafeConnectAsync(ConnectContext ctx)
        {
            while (!ctx.IsConnected && ctx.Attempts < MaxAttempts && !ctx.CancellationToken.IsCancellationRequested) ;
            {
                ctx.Attempts++;
                await DoSafeConnectAsync(ctx);
            }
        }

        private async Task SafeReadAsync(SendReceiveContext ctx)
        {
            while (!ctx.IsSuccess && ctx.Attempts < MaxAttempts && !ctx.CancellationToken.IsCancellationRequested)
            {
                if (ctx.IsDisconnected)
                {
                    var connectCtx = new ConnectContext
                    {
                        IsConnected = false,
                        Attempts = 0
                    };
                    await SafeConnectAsync(connectCtx);
                    if (connectCtx.IsConnected)
                    {
                        ctx.IsDisconnected = false;
                    }
                    else
                    {
                        return;
                    }
                }
                ctx.Attempts++;
                await DoSafeReadAsync(ctx);
            }
        }

        private async Task<Message> InnerReceiveMessage()
        {
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var headCtx = new SendReceiveContext
            {
                IsSuccess = false,
                IsDisconnected = false,
                Data = new byte[MessageHead.HeadLenght],
                Attempts = 0,
                TimeoutDateTime = DateTime.Now.AddMilliseconds(Timeout),
                CancellationToken = timeoutCancellation.Token
            };
            await SafeReadAsync(headCtx);
            if (headCtx.IsSuccess)
            {
                MessageHead? head = null;
                try
                {
                    head = MessageHead.Parse(headCtx.Data);
                }
                catch
                {
                    throw new Exception("Head parse fail!");
                }
                var dataCtx = new SendReceiveContext
                {
                    IsSuccess = false,
                    IsDisconnected = false,
                    Data = new byte[head.Lenght],
                    Attempts = 0,
                    TimeoutDateTime = headCtx.TimeoutDateTime,
                    CancellationToken = headCtx.CancellationToken
                };
                await SafeReadAsync(dataCtx);
                if (dataCtx.IsSuccess)
                {
                    return new Message(head, dataCtx.Data);
                }
                else
                {
                    if (dataCtx.Exception != null)
                    {
                        throw dataCtx.Exception;
                    }
                    else if (dataCtx.Attempts >= MaxAttempts)
                    {
                        throw new ArgumentOutOfRangeException($"Connection attepts: {headCtx.Attempts}");
                    }
                    else if (DateTime.Now >= dataCtx.TimeoutDateTime)
                    {
                        throw new TimeoutException($"Connection time: {DateTime.Now - headCtx.TimeoutDateTime}");
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }
            }
            else
            {
                if (headCtx.Exception != null)
                {
                    throw headCtx.Exception;
                }
                else if (headCtx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {headCtx.Attempts}");
                }
                else if (DateTime.Now >= headCtx.TimeoutDateTime)
                {
                    throw new TimeoutException($"Connection time: {DateTime.Now - headCtx.TimeoutDateTime}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private async Task SafeSendAsync(SendReceiveContext ctx)
        {
            while (!ctx.IsSuccess && ctx.Attempts < MaxAttempts && !ctx.CancellationToken.IsCancellationRequested)
            {
                if (ctx.IsDisconnected)
                {
                    var connectCtx = new ConnectContext
                    {
                        IsConnected = false,
                        Attempts = 0,
                        TimeoutDateTime = ctx.TimeoutDateTime,
                        CancellationToken = ctx.CancellationToken
                    };
                    await SafeConnectAsync(connectCtx);
                    if (connectCtx.IsConnected)
                    {
                        ctx.IsDisconnected = false;
                    }
                    else
                    {
                        return;
                    }
                }
                ctx.Attempts++;
                await DoSafeSendAsync(ctx);
            }
        }

        private async Task InnerSendMessage(Message message)
        {
            var data = message.Head
                .GetBytes()
                .Concat(message.Data)
                .ToArray();
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var ctx = new SendReceiveContext
            {
                IsSuccess = false,
                IsDisconnected = !IsConnected(),
                Data = data,
                Attempts = 0,
                TimeoutDateTime = DateTime.Now.AddMilliseconds(Timeout),
                CancellationToken = timeoutCancellation.Token
            };
            await SafeSendAsync(ctx);
            if (ctx.IsSuccess)
            {
                return;
            }
            else
            {
                if (ctx.Exception != null)
                {
                    throw ctx.Exception;
                }
                else if (ctx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {ctx.Attempts}");
                }
                else if (DateTime.Now >= ctx.TimeoutDateTime)
                {
                    throw new TimeoutException($"Connection time: {DateTime.Now - ctx.TimeoutDateTime}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        public async Task<Message> SendAndReceive(Message message)
        {
            await InnerSendMessage(message);
            return await InnerReceiveMessage();
        }

        public Task SendMessage(Message message)
        {
            return InnerSendMessage(message);
        }

        public Task<Message> ReceiveMessage()
        {
            return InnerReceiveMessage();
        }
    }
}
