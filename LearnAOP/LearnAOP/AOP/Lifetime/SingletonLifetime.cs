using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP.Lifetime
{
    public class SingletonLifetime : ILifetime
    {
        private Object _lockGet;
        private IDictionary<uint, object> _instances;

        public SingletonLifetime()
        {
            _lockGet = new Object();
            _instances = new Dictionary<uint, object>();
        }

        public object GetInstance(ContainerType containerType)
        {
            lock (_lockGet)
            {
                if (!_instances.ContainsKey(containerType.Index))
                {
                    var newInstance = containerType.Builder.Generate(containerType);

                    _instances[containerType.Index] = newInstance;
                }
            }
            return _instances[containerType.Index];
        }
    }
}