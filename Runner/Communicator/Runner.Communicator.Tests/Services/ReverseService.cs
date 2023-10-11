using Runner.Communicator.Tests.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Services
{
    public class ReverseService : IReverseInterface
    {
        public Task<string> Pong()
        {
            return Task.FromResult("PING");
        }
    }
}
