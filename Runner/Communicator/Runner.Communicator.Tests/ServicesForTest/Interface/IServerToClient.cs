using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.ServicesForTest.Interface
{
    public interface IClientToServer
    {
        Task<string> ClientToServer();
    }

    public interface IServerToClient
    {
        Task<string> ServerToClient();
        Task<string> WaitServerCall();
    }
}
