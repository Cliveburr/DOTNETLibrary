using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Runner.Communication.Model;

namespace Runner.Communication
{
    public abstract class SocketTcp : SocketCon
    {
        protected TcpClient? _tcpClient;
        private int _interTimeoutMilliseconds;

        public SocketTcp(TcpClient? tcpClient, int maxAttempts)
            : base(maxAttempts)
        {
            _tcpClient = tcpClient;
        }

        public SocketTcp(int maxAttempts)
            : this(null, maxAttempts)
        {
        }

        protected override async Task DoReadAsync(SendReceiveContext ctx)
        {
            try
            {
                if (_tcpClient != null)
                {
                    await _tcpClient.GetStream().ReadExactlyAsync(ctx.Data, 0, ctx.Data.Length, CancellationToken);
                    ctx.IsSuccess = true;
                }
            }
            catch (EndOfStreamException err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (IOException err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (OperationCanceledException err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (Exception err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
            }
        }

        protected override async Task DoSendAsync(SendReceiveContext ctx)
        {
            try
            {
                if (_tcpClient != null)
                {
                    await _tcpClient.GetStream().WriteAsync(ctx.Data, CancellationToken);
                    ctx.IsSuccess = true;
                }
            }
            catch (EndOfStreamException err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (IOException err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (OperationCanceledException err)
            {
                if(ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
                ctx.IsDisconnected = true;
            }
            catch (Exception err)
            {
                if (ctx.Exception == null)
                {
                    ctx.Exception = err;
                }
            }
        }
    }
}
