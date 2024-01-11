using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Authentication
{
    public enum AccessTokenType : byte
    {
        WebUI = 0,
        Agent = 1
    }
}
