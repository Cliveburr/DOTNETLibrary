using DependencyResolver.Builder.Common;
using DependencyResolver.Containers;
using DependencyResolver.Helpers;
using DependencyResolver.Proxy.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Proxy.Builder
{
    public class ProxyObject
    {
        private Type _proxyGenericType;
        private CommonBuilderConstructor _constructor;
        private Dictionary<MethodInfo, ProxyObjectEvents> _interceptions;

        public ProxyObject(Type serviceType, Type implementationType, List<IInterceptionQuery> interceptions)
        {
            _constructor = new CommonBuilderConstructor();
            var proxyType = typeof(ProxyObject<>);
            _proxyGenericType = proxyType.MakeGenericType(serviceType);
            _interceptions = GenerateInterceptions(serviceType, implementationType, interceptions);
        }

        private Dictionary<MethodInfo, ProxyObjectEvents> GenerateInterceptions(Type serviceType, Type implementationType, List<IInterceptionQuery> interceptions)
        {
            var result = new Dictionary<MethodInfo, ProxyObjectEvents>();
            var serviceMethods = serviceType.GetMethods();
            var globalAttribute = AttributeHelper.GetAttributes<InterceptionAttribute>(implementationType, serviceType)
                .ToList();

            var methods = MapMethods(serviceType, implementationType);

            foreach (var method in methods)
            {
                var key = method.Item1;

                var interceptionsMethods = AttributeHelper.GetAttributes<InterceptionAttribute>(method.Item1)
                    .Select(a => a.Events);

                interceptionsMethods = interceptionsMethods.Concat(interceptions
                    .Where(i => i.IsApply(method.Item1))
                    .Select(i => i.GetEvents(method.Item1)));

                if (method.Item2 != null)
                {
                    key = method.Item2;

                    interceptionsMethods = interceptionsMethods.Concat(AttributeHelper.GetAttributes<InterceptionAttribute>(method.Item2)
                        .Select(a => a.Events));

                    interceptionsMethods = interceptionsMethods.Concat(interceptions
                        .Where(i => i.IsApply(method.Item2))
                        .Select(i => i.GetEvents(method.Item2)));
                }

                var all = interceptionsMethods
                    .SelectMany(e => e)
                    .Distinct()
                    .ToArray();
                if (all.Any())
                {
                    result.Add(key, SepareteEvents(all));
                }
            }
            return result;
        }

        private Tuple<MethodInfo, MethodInfo>[] MapMethods(Type serviceType, Type implementationType)
        {
            var map = implementationType.GetInterfaceMap(serviceType);

            return implementationType.GetMethods()
                .Select(m =>
                {
                    var index = Array.IndexOf(map.TargetMethods, m);
                    if (index > -1)
                    {
                        return new Tuple<MethodInfo, MethodInfo>(m, map.InterfaceMethods[index]);
                    }
                    else
                    {
                        return new Tuple<MethodInfo, MethodInfo>(m, null);
                    }
                })
                .ToArray();
        }

        private ProxyObjectEvents SepareteEvents(IInterceptEvent[] all)
        {
            var preEvents = all
                .OfType<IInterceptPreEvent>()
                .ToArray();

            var posEvents = all
                .OfType<IInterceptPosEvent>()
                .ToArray();

            var errorEvents = all
                .OfType<IInterceptErrorEvent>()
                .ToArray();

            return new ProxyObjectEvents
            {
                PreEvents = preEvents,
                PosEvents = posEvents,
                ErrorEvents = errorEvents
            };
        }

        public object Instantiate(Type implementationType, ResolveContext context)
        {
            var obj = _constructor.Instantiate(implementationType, context);
            return _proxyGenericType.InvokeMember("Create", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { obj, _interceptions });
        }
    }

    public class ProxyObjectEvents
    {
        public IInterceptPreEvent[] PreEvents { get; set; }
        public IInterceptPosEvent[] PosEvents { get; set; }
        public IInterceptErrorEvent[] ErrorEvents { get; set; }
    }

    public class ProxyObject<T> : DispatchProxy
    {
        private T _object;
        private Dictionary<MethodInfo, ProxyObjectEvents> _events;

        public static T Create(T decorated, Dictionary<MethodInfo, ProxyObjectEvents> events)
        {
            object proxy = Create<T, ProxyObject<T>>();
            ((ProxyObject<T>)proxy).Initialize(decorated, events);
            return (T)proxy;
        }

        public void Initialize(T obj, Dictionary<MethodInfo, ProxyObjectEvents> events)
        {
            _object = obj;
            _events = events;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (_events.ContainsKey(targetMethod))
            {
                var events = _events[targetMethod];
                try
                {
                    if (events.PreEvents.Any())
                    {
                        RunPreEvents(events.PreEvents, targetMethod, args);
                    }

                    var result = targetMethod.Invoke(_object, args);

                    if (events.PosEvents.Any())
                    {
                        RunPosEvents(events.PosEvents, targetMethod, args);
                    }

                    return result;
                }
                catch (Exception ex) when (ex is TargetInvocationException)
                {
                    var err = ex.InnerException ?? ex;

                    if (events.PosEvents.Any())
                    {
                        RunErrorEvents(events.ErrorEvents, err, targetMethod, args);
                        return null;
                    }
                    else
                    {
                        throw err;
                    }
                }
            }
            else
            {
                return targetMethod.Invoke(_object, args);
            }
        }

        private void RunPreEvents(IInterceptPreEvent[] events, MethodInfo targetMethod, object[] args)
        {
            var context = new InterceptPreEventContext
            {
                Method = targetMethod,
                Arguments = args
            };

            foreach (var evnt in events)
            {
                evnt.PreEvent(context);
            }
        }

        private void RunPosEvents(IInterceptPosEvent[] events, MethodInfo targetMethod, object[] args)
        {
            var context = new InterceptPosEventContext
            {
                Method = targetMethod,
                Arguments = args
            };

            foreach (var evnt in events)
            {
                evnt.PosEvent(context);
            }
        }

        private void RunErrorEvents(IInterceptErrorEvent[] events, Exception err, MethodInfo targetMethod, object[] args)
        {
            var context = new InterceptErrorEventContext
            {
                Method = targetMethod,
                Arguments = args,
                RaiseException = true,
                Exception = err
            };

            foreach (var evnt in events)
            {
                evnt.ErrorEvent(context);
            }

            if (context.RaiseException)
            {
                throw context.Exception;
            }
        }
    }
}