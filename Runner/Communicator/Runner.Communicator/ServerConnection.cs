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
using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
//using Runner.Communicator.Process.FileUpload;
using Runner.Communicator.Process.Services;

namespace Runner.Communicator
{
    public class ServerConnection : Abstract.SocketTcp
    {
        public delegate void OnCloseDelegate(ServerConnection sender);
        public event OnCloseDelegate? OnClose;
        public delegate void OnErrorDelegate(ServerConnection sender, Exception err);
        public event OnErrorDelegate? OnError;

        public ushort Id { get; private set; }

        private Server _server;
        private ManualResetEvent? _waitReconnect;
        //private ProcessServices? _processServices;
        //private ProcessFileUpload? _processFileUpload;

        public ServerConnection(TcpClient tcpClient, Server server, ushort id, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken)
        {
            _server = server;
            Id = id;
        }

        public void ReplaceTcpClient(TcpClient tcpClient)
        {
            DisconnectSocket();
            _tcpClient = tcpClient;
            _waitReconnect?.Set();
        }

        protected override Task DoConnectAsync(CancellationToken cancellationToken)
        {
            _waitReconnect = new ManualResetEvent(false);
            WaitHandle.WaitAny(new WaitHandle[]
            {
                cancellationToken.WaitHandle,
                _waitReconnect
            });
            cancellationToken.ThrowIfCancellationRequested();
            //cancellationToken.WaitHandle.WaitOne()

            //if (_waitReconnect .WaitOne() .WaitOne(60000))  // 3 * 60 * 1000
            //{
            //    ctx.IsConnected = true;
            //}
            _waitReconnect = null;
            //if (ctx.Attempts >= MaxAttempts)
            //{
            //    _ = Task.Run(() => OnClose?.Invoke(this));
            //}
            return Task.CompletedTask;
        }


        //public void Start()
        //{
        //    Task.Run(StartAsync);
        //}

        //private async Task StartAsync()
        //{
        //    while (!_server.CancellationToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            var message = await ReceiveMessage();

        //            var response = await ProcessRequestAsync(message);
        //            await SendMessage(response);
        //        }
        //        catch (Exception err)
        //        {
        //            _ = Task.Run(() => OnError?.Invoke(this, err));
        //        }
        //    }
        //}

        protected override Task<byte[]?> DoProcessRequest(byte[] data, MessagePort port)
        {
            switch (port)
            {
                //case MessagePort.HandShake:
                default: return null;
            }
        }

        //private Task<Message> ProcessRequestAsync(Message message)
        //{
        //    switch (message.Head.Type)
        //    {
        //        case MessagePort.Services:
        //            return GetProcessServices().ProcessRequest(message);
        //        //case MessageType.FileUpload:
        //        //    return GetProcessFileUpload().ProcessRequest(message);
        //        default:
        //            throw new Exception("Invalid message type: " + message.Head.Type);
        //    }
        //}

        //private ProcessServices GetProcessServices()
        //{
        //    if (_processServices == null)
        //    {
        //        _processServices = new ProcessServices(_server);
        //    }
        //    return _processServices;
        //}

        //private ProcessFileUpload GetProcessFileUpload()
        //{
        //    if (_processFileUpload == null)
        //    {
        //        _processFileUpload = new ProcessFileUpload(_server);
        //    }
        //    return _processFileUpload;
        //}

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
