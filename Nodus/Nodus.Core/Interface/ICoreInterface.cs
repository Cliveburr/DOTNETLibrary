using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Interface
{
    [ServiceContract()]
    public interface ICoreInterface
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool Ping();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Version Version();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void RouteTo(string host, int port);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void Update();
    }
}