using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Interfaces
{
    public interface IReverseInterface
    {
        Task<string> Pong();
    }
}
