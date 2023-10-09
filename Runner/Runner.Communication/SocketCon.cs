using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Runner.Communication.Helpers;
using Runner.Communication.Model;
using Runner.Communication.Process.Services;

namespace Runner.Communication
{
    public abstract class SocketCon
    {
        public CancellationToken CancellationToken { get; set; }
        public int MaxAttempts { get; set; }

        protected abstract Task DoConnectAsync(ConnectContext ctx);
        protected abstract Task DoSendAsync(SendReceiveContext ctx);
        protected abstract Task DoReadAsync(SendReceiveContext ctx);

        protected SocketCon(int maxAttempts)
        {
            MaxAttempts = maxAttempts;
        }

        protected class SendReceiveContext
        {
            public bool IsSuccess { get; set; }
            public bool IsDisconnected { get; set; }
            public required byte[] Data { get; set; }
            public int Attempts { get; set; }
            public Exception? Exception { get; set; }
        }

        protected class ConnectContext
        {
            public bool IsConnected { get; set; }
            public int Attempts { get; set; }
            public Exception? Exception { get; set; }
        }

        public async Task ConnectAsync()
        {
            var ctx = new ConnectContext
            {
                IsConnected = false,
                Attempts = 0
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
                //else if (DateTime.Now >= ctx.TimeoutDateTime)
                //{
                //    throw new TimeoutException($"Connection time: {DateTime.Now - ctx.TimeoutDateTime}");
                //}
                else if (ctx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {ctx.Attempts}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private async Task SafeConnectAsync(ConnectContext ctx)
        {
            while (!ctx.IsConnected && ctx.Attempts < MaxAttempts && !CancellationToken.IsCancellationRequested)
            {
                ctx.Attempts++;
                await DoConnectAsync(ctx);
            }
        }

        private async Task SafeReadAsync(SendReceiveContext ctx)
        {
            while (!ctx.IsSuccess && ctx.Attempts < MaxAttempts && !CancellationToken.IsCancellationRequested)
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
                await DoReadAsync(ctx);
            }
        }

        private async Task<Message> InnerReceiveMessage()
        {
            var headCtx = new SendReceiveContext
            {
                IsSuccess = false,
                IsDisconnected = false,
                Data = new byte[MessageHead.HeadLenght],
                Attempts = 0
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
                    Attempts = 0
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
                    //else if (DateTime.Now >= dataCtx.TimeoutDateTime)
                    //{
                    //    throw new TimeoutException($"Connection time: {DateTime.Now - headCtx.TimeoutDateTime}");
                    //}
                    else if (dataCtx.Attempts >= MaxAttempts)
                    {
                        throw new ArgumentOutOfRangeException($"Connection attepts: {headCtx.Attempts}");
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
                //else if (DateTime.Now >= headCtx.TimeoutDateTime)
                //{
                //    throw new TimeoutException($"Connection time: {DateTime.Now - headCtx.TimeoutDateTime}");
                //}
                else if (headCtx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {headCtx.Attempts}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private async Task SafeSendAsync(SendReceiveContext ctx)
        {
            while (!ctx.IsSuccess && ctx.Attempts < MaxAttempts && !CancellationToken.IsCancellationRequested)
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
                await DoSendAsync(ctx);
            }
        }

        private async Task InnerSendMessage(Message message)
        {
            var data = message.Head
                .GetBytes()
                .Concat(message.Data)
                .ToArray();
            var ctx = new SendReceiveContext
            {
                IsSuccess = false,
                IsDisconnected = false,
                Data = data,
                Attempts = 0
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
                //else if (DateTime.Now >= ctx.TimeoutDateTime)
                //{
                //    throw new TimeoutException($"Connection time: {DateTime.Now - ctx.TimeoutDateTime}");
                //}
                else if (ctx.Attempts >= MaxAttempts)
                {
                    throw new ArgumentOutOfRangeException($"Connection attepts: {ctx.Attempts}");
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }

        public async Task<Message> SendRequest(Message request)
        {
            await InnerSendMessage(request);

            return await InnerReceiveMessage();
        }

        public Task SendResponse(Message request)
        {
            return InnerSendMessage(request);
        }

        public Task<Message> ReceiveMessage()
        {
            return InnerReceiveMessage();
        }
    }
}
