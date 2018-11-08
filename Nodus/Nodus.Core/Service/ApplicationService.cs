using Nodus.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Nodus.Core.Model.Application;

namespace Nodus.Core.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ApplicationService : ClientBase<IApplicationInterface>, IApplicationInterface, IDisposable
    {
        internal ApplicationService(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public void RouteTo(string host, int port)
        {
            Channel.RouteTo(host, port);
        }

        public string Load(string script)
        {
            return Channel.Load(script);
        }

        public Result Run(string token)
        {
            return Channel.Run(token);
        }

        public Result Run2(string scriptFile, string function, params object[] arguments)
        {
            return Channel.Run2(scriptFile, function, arguments);
        }
    }
}