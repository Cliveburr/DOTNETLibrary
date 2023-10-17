using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Tests.Interfaces;

namespace Runner.Communicator.Tests.Services
{
    public class WaiterService : IWaiterInterface
    {
        public Task<string> Wait(int seconds)
        {
            return new Task<string>(new Func<string>(() =>
            {
                Thread.Sleep(seconds * 1000);
                return seconds.ToString();
            }));
        }
    }
}
