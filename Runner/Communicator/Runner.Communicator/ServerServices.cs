using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator
{
    public class ServerServices
    {
        public required Type Interface { get; set; }
        public required Type Implementation { get; set; }
    }
}
