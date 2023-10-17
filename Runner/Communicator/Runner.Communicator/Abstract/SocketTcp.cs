using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Model;

namespace Runner.Communicator.Abstract
{
    public abstract class SocketTcp : SocketBase
    {
        protected TcpClient? _tcpClient;

        public SocketTcp(TcpClient? tcpClient, CancellationToken cancellationToken = new CancellationToken())
            : base(60000, cancellationToken)
        {
            _tcpClient = tcpClient;
        }

        protected override bool IsConnected()
        {
            return _tcpClient != null;
        }

        protected void DisconnectSocket()
        {
            try
            {
                _tcpClient?.Close();
            }
            catch { }
            try
            {
                _tcpClient?.Dispose();
            }
            catch { }
            _tcpClient = null;
        }

        public override void Dispose()
        {
            base.Dispose();
            DisconnectSocket();
        }

        protected override async Task<byte[]> DoReadAsync(CancellationToken cancellationToken, uint length)
        {
            if (_tcpClient == null)
            {
                throw new Exception("Not connected!");
            }
            try
            {
                var data = new byte[length];
                await _tcpClient.GetStream().ReadExactlyAsync(data, 0, (int)length, cancellationToken);
                return data;
            }
            catch (EndOfStreamException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch (IOException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch (OperationCanceledException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch
            {
                throw;
            }
        }

        protected override async Task DoSendAsync(CancellationToken cancellationToken, byte[] data)
        {
            if (_tcpClient == null)
            {
                throw new Exception("Not connected!");
            }
            try
            {
                await _tcpClient.GetStream().WriteAsync(data, cancellationToken);
            }
            catch (EndOfStreamException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch (IOException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch (OperationCanceledException err)
            {
                DisconnectSocket();
                throw err;
            }
            catch
            {
                throw;
            }
        }
    }
}
