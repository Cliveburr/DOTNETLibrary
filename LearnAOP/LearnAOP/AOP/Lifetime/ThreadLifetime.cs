using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LearnAOP.AOP.Lifetime
{
    public class ThreadLifetime : ILifetime
    {
        private ThreadLocal<object> _instance;

        public ThreadLifetime()
        {
            _instance = new ThreadLocal<object>();
        }

        public object GetInstance(ContainerType containerType)
        {
            if (!_instance.IsValueCreated)
            {
                _instance.Value = containerType.Builder.Generate(containerType);
            }
            return _instance.Value;
        }
    }
}