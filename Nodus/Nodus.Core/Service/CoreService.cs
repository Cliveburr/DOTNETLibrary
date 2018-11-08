using Nodus.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal class CoreService : ClientBase<ICoreInterface>, ICoreInterface
    {
        internal CoreService(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public bool Ping()
        {
            return Channel.Ping();
        }

        public Version Version()
        {
            return Channel.Version();
        }

        public void RouteTo(string host, int port)
        {
            Channel.RouteTo(host, port);
        }

        public void Update()
        {
            Channel.Update();
        }
    }
}