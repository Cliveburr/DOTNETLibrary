using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Runner.Communication.FileUpload;
using Runner.Communication.Helpers;
using Runner.Communication.Model;
using Runner.Communication.Process.Services;

namespace Runner.Communication
{
    public class Client : SocketTcp, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private ClientServices? _clientServices;
        private string _hostname;
        private int _port;
        private ushort _id;

        public Client(CancellationTokenSource cancellationTokenSource, string hostname, int port, int maxAttempts)
            : base(maxAttempts)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _hostname = hostname;
            _port = port;
            _id = 0;
        }

        public Client(string hostname, int port)
            : this(new CancellationTokenSource(), hostname, port, 3)
        {
        }

        public static async Task<Client> Connect(string hostname, int port)
        {
            var client = new Client(hostname, port);
            await client.ConnectAsync();
            return client;
        }

        public void Dispose()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch { }
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
        }

        protected override async Task DoConnectAsync(ConnectContext ctx)
        {
            try
            {
                try
                {
                    _tcpClient?.Close();
                }
                catch { }
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_hostname, _port, CancellationToken);
                ctx.IsConnected = true;
            }
            catch (Exception err)
            {
                if (ctx.Exception != null)
                {
                    ctx.Exception = err;
                }
            }

            if (ctx.IsConnected)
            {
                try
                {
                    await ShakeHand();
                }
                catch (Exception err)
                {
                    ctx.IsConnected = false;
                    if (ctx.Exception != null)
                    {
                        ctx.Exception = err;
                    }
                }
            }
        }

        private async Task ShakeHand()
        {
            var writer = new BytesWriter();
            writer.WriteUInt16(_id);
            writer.WriteUInt16(1234);
            var request = new Message(MessageType.HandShake, writer.GetBytes());

            var response = await SendRequest(request);

            if (response == null)
            {
                throw new Exception("ShakeHand fail!");
            }
            if (response.Head.Type != MessageType.HandShake)
            {
                throw new Exception("ShakeHand response wrong!");
            }

            var reader = new BytesReader(response.Data);
            var id = reader.ReadUInt16();
            var ack = reader.ReadUInt16();
            if (ack != 1234)
            {
                throw new Exception("ShakeHand ack wrong!");
            }

            _id = id;
        }

        public T Open<T>()
        {
            if (_clientServices == null)
            {
                _clientServices = new ClientServices();
            }
            return _clientServices.Open<T>(this);
        }

        public ClientFileUpload FileUpload(string localFilePath, string destineFilePath, bool overwrite = false, int partSizeMB = 10)
        {
            return new ClientFileUpload(this, localFilePath, destineFilePath, overwrite, partSizeMB);
        }

        public Task DeleteFolder(string folder)
        {
            var clientFileUpload = new ClientFileUpload(this, "", "", false);
            return clientFileUpload.DeleteFolder(new Process.FileUpload.Model.DeleteFolderRequest
            {
                Folder = folder
            });
        }
    }
}
