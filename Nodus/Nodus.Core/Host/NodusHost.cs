using Nodus.Core.Helper;
using Nodus.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Host
{
    public class NodusHost
    {
        public static NodusHost Instance { get; private set; }
        public delegate void CloseEvent();
        public event CloseEvent OnClose;
        public int Port { get; private set; }

        private ServiceHost _coreHost;
        private ServiceHost _ioHost;
        private ServiceHost _applicationHost;

        public NodusHost(int port)
        {
            Port = port;
            Instance = this;
        }

        public void Start()
        {
            _coreHost = TcpDefaults.CreateService<CoreHost, ICoreInterface>($"net.tcp://localhost:{Port}/Nodus/Core");
            _ioHost = TcpDefaults.CreateService<IOHost, IIOInterface>($"net.tcp://localhost:{Port}/Nodus/IO");
            _applicationHost = TcpDefaults.CreateService<ApplicationHost, IApplicationInterface>($"net.tcp://localhost:{Port}/Nodus/Application");
        }

        public void Close()
        {
            try { _coreHost.Close(); } catch { };
            try { _ioHost.Close(); } catch { };
            try { _applicationHost.Close(); } catch { };
        }

        public void Terminate()
        {
            OnClose?.Invoke();
        }
    }
}