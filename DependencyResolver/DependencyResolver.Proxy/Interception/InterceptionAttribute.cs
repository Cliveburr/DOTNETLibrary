using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Proxy.Interception
{
    public class InterceptionAttribute : Attribute
    {
        public IInterceptEvent[] Events { get; }
    }
}