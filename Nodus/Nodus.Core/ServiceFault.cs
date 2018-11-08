using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core
{
    public class ServiceFault : FaultException
    {
        public Exception OriginalException { get; private set; }

        public ServiceFault()
        {
        }

        public ServiceFault(string msg)
            : base(msg)
        {
        }

        public ServiceFault(string msg, Exception err)
            : base(msg)
        {
            OriginalException = err;
        }


        public ServiceFault(string msg, params string[] args)
            : base(string.Format(msg, args))
        {
        }
    }
}