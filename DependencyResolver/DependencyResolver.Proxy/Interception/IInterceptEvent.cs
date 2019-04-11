using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Proxy.Interception
{
    public interface IInterceptEvent
    {
    }

    public interface IInterceptPreEvent : IInterceptEvent
    {
        void PreEvent(InterceptPreEventContext context);
    }

    public interface IInterceptPosEvent : IInterceptEvent
    {
        void PosEvent(InterceptPosEventContext context);
    }

    public interface IInterceptErrorEvent : IInterceptEvent
    {
        void ErrorEvent(InterceptErrorEventContext context);
    }
}