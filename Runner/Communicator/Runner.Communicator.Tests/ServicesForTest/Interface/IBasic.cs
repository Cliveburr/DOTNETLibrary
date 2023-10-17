using Runner.Communicator.Tests.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.ServicesForTest.Interface
{
    public interface IBasic
    {
        Task<string> Ping();
        Task PrimitiveParameters(bool bol, string str, int v1, short v2, long v3);
        Task<OneModel> ComplexModel(OneModel model);
        Task GetTimeout(int milliseconds);
        Task Voidtask();
        Task NormalException();
        Task NullReferenceException();
    }
}
