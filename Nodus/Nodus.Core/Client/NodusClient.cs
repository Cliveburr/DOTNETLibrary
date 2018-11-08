using Nodus.Core.Helper;
using Nodus.Core.Model.Core;
using Nodus.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Client
{
    public partial class NodusClient : IDisposable
    {
        public delegate void DisposeEvent();
        public event DisposeEvent OnDispose;
        public List<Hoop> Hoops { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        internal CoreService _core;
        internal IOService _IO;
        internal ApplicationService _application;

        public NodusClient(string host, int port)
        {
            Host = host;
            Port = port;
            Hoops = new List<Hoop>
            {
                new Hoop
                {
                    Host = host,
                    Port = port,
                    Index = 0
                }
            };
        }

        public void Start()
        {
            _core = new CoreService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{Host}:{Port}/Nodus/Core"));
            _core.InnerChannel.Closed += InnerChannel_Closed;
            _IO = new IOService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{Host}:{Port}/Nodus/IO"));
            _IO.InnerChannel.Closed += InnerChannel_Closed;
            _application = new ApplicationService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{Host}:{Port}/Nodus/Application"));
            _application.InnerChannel.Closed += InnerChannel_Closed;
        }

        private void InnerChannel_Closed(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            try { _core.Close(); } catch { };
            try { _IO.Close(); } catch { };
            try { _application.Close(); } catch { };
            OnDispose?.Invoke();
        }
    }
}