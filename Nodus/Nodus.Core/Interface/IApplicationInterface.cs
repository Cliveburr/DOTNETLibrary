using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Interface
{
    [ServiceContract()]
    public interface IApplicationInterface
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void RouteTo(string host, int port);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        string Load(string script);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Result Run(string token);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Result Run2(string scriptFile, string function, params object[] arguments);
    }
}