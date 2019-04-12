using DependencyResolver.Proxy.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Tests.Proxy
{
    public class LogInterception : IInterceptionQuery, IInterceptPreEvent
    {
        public List<string> Logs { get; }

        public LogInterception()
        {
            Logs = new List<string>();
        }

        public IInterceptEvent[] GetEvents(MethodInfo method)
        {
            return new IInterceptEvent[]
            {
                this
            };
        }

        public bool IsApply(MethodInfo method)
        {
            return true;
        }

        public void PreEvent(InterceptPreEventContext context)
        {
            var text = $"Method: {context.Method.Name} with arguments {string.Join(" ,", context.Arguments.Select(a => a.ToString()))}";

            Logs.Add(text);
        }
    }
}