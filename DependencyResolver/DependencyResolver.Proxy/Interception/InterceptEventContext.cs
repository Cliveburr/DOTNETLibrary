using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Proxy.Interception
{
    public class InterceptPreEventContext
    {
        public MethodInfo Method { get; set; }
        public object[] Arguments { get; set; }
    }

    public class InterceptPosEventContext
    {
        public MethodInfo Method { get; set; }
        public object[] Arguments { get; set; }
    }

    public class InterceptErrorEventContext
    {
        public MethodInfo Method { get; set; }
        public object[] Arguments { get; set; }
        public bool RaiseException { get; set; }
        public Exception Exception { get; set; }
    }
}