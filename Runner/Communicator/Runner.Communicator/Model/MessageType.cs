using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Model
{
    public enum MessagePort : ushort
    {
        Any = 0,
        HandShake = 1,
        Services = 2,
        FileUpload = 3
    }
}
