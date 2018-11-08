using Nodus.Core.Application.Tag;
using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodus.Core.Application
{
    public class AppInstance : IDisposable
    {
        public AppDomain Domain { get; set; }
        public string Token { get; set; }
        public DomainInterop DomainInterop { get; set; }

        public void Dispose()
        {
            if (Domain != null)
                AppDomain.Unload(Domain);
            Domain = null;
            Token = null;
        }
    }
}