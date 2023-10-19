using Microsoft.AspNetCore.SignalR;
using Runner.Signal.WorkerInterfaceTests;

namespace Runner.Signal.WorkerServerTests.Service
{
    public class ClientToServer : Hub<IClientToServer>
    {
        public ClientToServer()
        {
            Console.Write("Construted");
        }

        public Task<string> Ping()
        {
            return Task.FromResult("PONG");
        }
    }
}
