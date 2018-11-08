using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Client
{
    public partial class NodusClient
    {
        public string Load(string script)
        {
            return _application.Load(script);
        }

        public Result Run(string token)
        {
            return _application.Run(token);
        }

        public Result Run2(string scriptFile, string function, params object[] arguments)
        {
            return _application.Run2(scriptFile, function, arguments);
        }
    }
}