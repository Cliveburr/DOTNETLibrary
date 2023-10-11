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
using Runner.Communicator.FileUpload;
using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;

namespace Runner.Communicator
{
    public class Client : Abstract.SocketTcp
    {
        private ClientServices? _clientServices;
        private string _hostname;
        private int _port;
        private ushort _id;

        public Client(string hostname, int port, CancellationToken cancellationToken = new CancellationToken())
            : base(null, cancellationToken)
        {
            _hostname = hostname;
            _port = port;
            _id = 0;
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

        protected override async Task DoConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpClient?.Close();
            }
            catch { }
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_hostname, _port, CancellationToken);
            await ShakeHand();
        }

        private async Task ShakeHand()
        {
            var writer = new BytesWriter();
            writer.WriteUInt16(_id);
            writer.WriteUInt16(1234);
            var request = new Message(MessageType.HandShake, writer.GetBytes());

            var response = await SendAndReceive(request);

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
