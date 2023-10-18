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
using System.Threading;

namespace Runner.Communicator
{
    public class ServerConnection : Abstract.SocketTcp
    {
        public delegate void OnCloseDelegate(ServerConnection sender);
        public event OnCloseDelegate? OnClose;

        public ushort Id { get; private set; }

        private ServiceCallerSocket? _serviceCaller;
        private ManualResetEvent? _waitReconnect;
        private IServiceScope _serviceScope;

        public ServerConnection(TcpClient tcpClient, ushort id, IServiceScope serviceScope, CancellationToken cancellationToken)
            : base(tcpClient, cancellationToken)
        {
            Id = id;
            _serviceScope = serviceScope;
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
            _waitReconnect = null;
            return Task.CompletedTask;
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

        //private ProcessFileUpload GetProcessFileUpload()
        //{
        //    if (_processFileUpload == null)
        //    {
        //        _processFileUpload = new ProcessFileUpload(_server);
        //    }
        //    return _processFileUpload;
        //}
    }
}
