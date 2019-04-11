using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Proxy.Interception
{
    public interface IInterceptionQuery
    {
        bool IsApply(MethodInfo method);
        IInterceptEvent[] GetEvents(MethodInfo method);
    }
}