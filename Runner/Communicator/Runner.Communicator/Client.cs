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
using Microsoft.Extensions.DependencyInjection;
//using Runner.Communicator.FileUpload;
using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;

namespace Runner.Communicator
{
    public class Client : Abstract.SocketTcp
    {
        private ServiceCallerSocket? _serviceCaller;
        private string _hostname;
        private int _port;
        private ushort _id;
        private IServiceScope? _serviceScope;

        private Client(string hostname, int port, IServiceScope? serviceScope, CancellationToken cancellationToken)
            : base(null, cancellationToken)
        {
            _hostname = hostname;
            _port = port;
            _serviceScope = serviceScope;
            _id = 0;
        }

        public static async Task<Client> Connect(string hostname, int port, IServiceScope? serviceScope, CancellationToken cancellationToken = new CancellationToken())
        {
            var client = new Client(hostname, port, serviceScope, cancellationToken);
            await client.ConnectAsync();
            return client;
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
            try
            {
                await ShakeHand();
            }
            catch
            {
                DisconnectSocket();
                throw;
            }
        }

        private async Task ShakeHand()
        {
            var writer = new BytesWriter();
            writer.WriteUInt16(_id);
            writer.WriteUInt16(1234);

            var sendTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            if (Timeout > 0)
            {
                sendTimeoutCancellation.CancelAfter(60000);
            }
            await DoSendAsync(sendTimeoutCancellation.Token, writer.GetBytes());
            
            uint responseLenght = 4; // (ushort = 2 byte) * 2
            var readTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);
            if (Timeout > 0)
            {
                readTimeoutCancellation.CancelAfter(60000);
            }
            var responseData = await DoReadAsync(readTimeoutCancellation.Token, responseLenght);
            if (responseData == null)
            {
                throw new Exception("ShakeHand fail!");
            }

            var reader = new BytesReader(responseData);
            var id = reader.ReadUInt16();
            var ack = reader.ReadUInt16();
            if (ack != 1234)
            {
                throw new Exception("ShakeHand ack wrong!");
            }

            _id = id;
        }

        protected override async Task<byte[]?> DoProcessRequest(byte[] data, MessagePort port)
        {
            switch (port)
            {
                case MessagePort.Services: return await Services.Process(data);
                default: return null;
            }
        }

        public ServiceCallerSocket Services
        {
            get
            {
                if (_serviceCaller == null)
                {
                    _serviceCaller = new ServiceCallerSocket(this, _serviceScope, CancellationToken);
                }
                return _serviceCaller;
            }
        }

        //public ClientFileUpload FileUpload(string localFilePath, string destineFilePath, bool overwrite = false, int partSizeMB = 10)
        //{
        //    return new ClientFileUpload(this, localFilePath, destineFilePath, overwrite, partSizeMB);
        //}

        //public Task DeleteFolder(string folder)
        //{
        //    var clientFileUpload = new ClientFileUpload(this, "", "", false);
        //    return clientFileUpload.DeleteFolder(new Process.FileUpload.Model.DeleteFolderRequest
        //    {
        //        Folder = folder
        //    });
        //}
    }
}
