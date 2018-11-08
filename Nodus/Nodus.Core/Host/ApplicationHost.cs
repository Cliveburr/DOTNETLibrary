using Nodus.Core.Helper;
using Nodus.Core.Interface;
using Nodus.Core.Model.Application;
using Nodus.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Host
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ApplicationHost : IApplicationInterface
    {
        private ApplicationService _route = null;

        public ApplicationHost()
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

        public void RouteTo(string host, int port)
        {
            if (_route == null)
            {
                ApplicationService troute = null;
                try
                {
                    troute = new ApplicationService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{host}:{port}/Nodus/Application"));
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

        public string Load(string script)
        {
            if (_route == null)
            {
                try
                {
                    return Application.AppController.Load(script).Token;
                }
                catch (Exception err)
                {
                    throw new ServiceFault("Error starting the app!", err);
                }
            }
            else
                return _route.Load(script);
        }

        public Result Run(string token)
        {
            if (_route == null)
            {
                try
                {
                    return Application.AppController.Run(token);
                }
                catch (Exception err)
                {
                    throw new ServiceFault("Error starting the app!", err);
                }
            }
            else
                return _route.Run(token);
        }

        public Result Run2(string scriptFile, string function, params object[] arguments)
        {
            if (_route == null)
            {
                try
                {
                    return new Application2.AppRunner().Run(scriptFile, function, arguments);
                }
                catch (Exception err)
                {
                    throw new ServiceFault("Error starting the app!", err);
                }
            }
            else
                return _route.Run2(scriptFile, function, arguments);
        }
    }
}
