using Nodus.Core.Client;
using Nodus.Core.Helper;
using Nodus.Core.Interface;
using Nodus.Core.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nodus.Core.Host
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class CoreHost : ICoreInterface
    {
        private CoreService _route = null;
        private Timer _toUpdate;

        public CoreHost()
        {
            OperationContext.Current.InstanceContext.Closed += InstanceContext_Closed;
            OperationContext.Current.InstanceContext.Faulted += InstanceContext_Closed;
        }

        private void InstanceContext_Closed(object sender, EventArgs e)
        {
            if (_route != null)
            {
                _route.Close();
                _route = null;
            }
        }

        public bool Ping()
        {
            if (_route == null)
                return true;
            else
                return _route.Ping();
        }

        public Version Version()
        {
            if (_route == null)
                return typeof(NodusClient).Assembly.GetName().Version;
            else
                return _route.Version();
        }

        public void RouteTo(string host, int port)
        {
            if (_route == null)
            {
                CoreService troute = null;
                try
                {
                    troute = new CoreService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{host}:{port}/Nodus/Core"));
                    troute.Ping();
                }
                catch (Exception err)
                {
                    throw new ServiceFault($@"Error routing from ""{Environment.MachineName}"" to ""{host}""!", err);
                }
                _route = troute;
            }
            else
                _route.RouteTo(host, port);
        }

        public void Update()
        {
            if (_route == null)
            {
                _toUpdate = new Timer(ToUpdateFunc, null, 300, Timeout.Infinite);
            }
            else
                _route.Update();
        }

        private void ToUpdateFunc(object state)
        {
            NodusHost.Instance.Close();

            var updateFolder = IO.PathCombine(IO.RootPath(), "Update");
            var updateFile = IO.PathCombine(updateFolder, "Nodus.Update.exe");

            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = updateFolder;
            process.StartInfo.FileName = updateFile;
            process.Start();

            NodusHost.Instance.Terminate();
        }
    }
}