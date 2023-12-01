using Runner.Agent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Isolation
{
    public class ScriptIsolation : IDisposable
    {
        private AppDomain? _domain;
        private ProxDomain? _proxy;

        public void Dispose()
        {
            _proxy = null;
            if (_domain is not null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }

        public Task<ExecuteResult> Execute(RunScriptRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
