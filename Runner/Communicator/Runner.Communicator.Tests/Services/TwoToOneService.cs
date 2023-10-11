using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Tests.Interfaces;

namespace Runner.Communicator.Tests.Services
{
    public class TwoToOneService : ITwoToOneInterface
    {
        public Task NormalException()
        {
            throw new Exception("NormalException");
        }

        public Task NullReferenceException()
        {
            throw new NullReferenceException("NullReferenceException");
        }

        public Task Extracheck(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            return Task.CompletedTask;
        }
    }
}
