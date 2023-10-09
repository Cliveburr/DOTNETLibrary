using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Runner.Communication.Helpers;
using Runner.Communication.Model;
using Runner.Communication.Process.FileUpload;
using Runner.Communication.Process.Services;

namespace Runner.Communication
{
    public class ServerConnection : SocketTcp
    {
        public delegate void OnCloseDelegate(ServerConnection sender);
        public event OnCloseDelegate? OnClose;
        public delegate void OnErrorDelegate(ServerConnection sender, Exception err);
        public event OnErrorDelegate? OnError;

        public ushort Id { get; set; }

        private Server _server;
        private ManualResetEvent? _waitReconnect;
        private ProcessServices? _processServices;
        private ProcessFileUpload? _processFileUpload;

        public ServerConnection(TcpClient tcpClient, Server server, int maxAttempts)
            : base(maxAttempts)
        {
            _tcpClient = tcpClient;
            _server = server;
            CancellationToken = server.CancellationToken;
        }

        public ServerConnection(TcpClient tcpClient, Server server)
            : this(tcpClient, server, 3)
        {
        }

        public void Close()
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
        }

        public void ReplaceTcpClient(TcpClient tcpClient)
        {
            Close();
            _tcpClient = tcpClient;
            _waitReconnect?.Set();
        }

        protected override Task DoConnectAsync(ConnectContext ctx)
        {
            _waitReconnect = new ManualResetEvent(false);
            if (_waitReconnect.WaitOne(60000))  // 3 * 60 * 1000
            {
                ctx.IsConnected = true;
            }
            _waitReconnect = null;
            if (ctx.Attempts >= MaxAttempts)
            {
                _ = Task.Run(() => OnClose?.Invoke(this));
            }
            return Task.CompletedTask;
        }

        public async Task<ushort> ShakeHand()
        {
            var response = await ReceiveMessage();
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

            if (id == 0)
            {
                id = _server.GetNextId();
            }

            var writer = new BytesWriter();
            writer.WriteUInt16(id);
            writer.WriteUInt16(1234);
            var request = new Message(MessageType.HandShake, writer.GetBytes());
            await SendResponse(request);

            return id;
        }


        public void Start()
        {
            Task.Run(StartAsync);
        }

        private async Task StartAsync()
        {
            while (!_server.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = await ReceiveMessage();

                    var response = await ProcessRequestAsync(message);
                    await SendResponse(response);
                }
                catch (Exception err)
                {
                    _ = Task.Run(() => OnError?.Invoke(this, err));
                }
            }
        }

        private Task<Message> ProcessRequestAsync(Message message)
        {
            switch (message.Head.Type)
            {
                case MessageType.Services:
                    return GetProcessServices().ProcessRequest(message);
                case MessageType.FileUpload:
                    return GetProcessFileUpload().ProcessRequest(message);
                default:
                    throw new Exception("Invalid message type: " + message.Head.Type);
            }
        }

        private ProcessServices GetProcessServices()
        {
            if (_processServices == null)
            {
                _processServices = new ProcessServices(_server);
            }
            return _processServices;
        }

        private ProcessFileUpload GetProcessFileUpload()
        {
            if (_processFileUpload == null)
            {
                _processFileUpload = new ProcessFileUpload(_server);
            }
            return _processFileUpload;
        }

        public void Stop()
        {
            try
            {
                _tcpClient?.Close();
                _tcpClient?.Dispose();
            }
            catch { }
        }
    }
}
