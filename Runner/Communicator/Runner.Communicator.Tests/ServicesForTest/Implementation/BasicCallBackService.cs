using Runner.Communicator.Process.Services;
using Runner.Communicator.Tests.ServicesForTest.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Services
{
    public class ClientToServerService : IClientToServer
    {
        private ServiceCallerBack<ServiceCallerImp> _caller;

        public ClientToServerService(ServiceCallerBack<ServiceCallerImp> caller)
        {
            _caller = caller;
        }

        public Task<string> ClientToServer()
        {
            var timer = new Timer(CallClient, null, 500, Timeout.Infinite);
            return Task.FromResult("SERVER");
        }

        private async void CallClient(object? state)
        {
            var serverToClient = _caller.Service.Open<IServerToClient>();
            var clientResponse = await serverToClient.ServerToClient();
            Assert.AreEqual(clientResponse, "CLIENT");
        }
    }

    public class ServerToClientService : IServerToClient
    {
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public Task<string> ServerToClient()
        {
            _manualResetEvent.Set();
            return Task.FromResult("CLIENT");
        }

        public Task<string> WaitServerCall()
        {
            return Task<string>.Run(() =>
            {
                _manualResetEvent.WaitOne();
                return "CLIENT";
            });
        }
    }
}
