using LearnAOP.AOP.Builder;
using LearnAOP.AOP.Factory;
using LearnAOP.AOP.Lifetime;
using LearnAOP.AOP.Pipeline;
using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP
{
    public class Container : IDisposable
    {
        public PipelineList Resolvers { get; private set; }
        public IFactory<ILifetime> DefaultLifetime { get; set; }
        public IFactory<IBuilder> DefaultBuilder { get; set; }
        public IList<InterceptionQuery> Interception { get; private set; }

        private IDictionary<string, ContainerType> _types;
        private Object _lockResolve;
        private uint _containerTypesIndex;

        public Container()
        {
            Resolvers = new PipelineList();
            DefaultLifetime = new SingletonFactory<ILifetime, TransientLifetime>();
            DefaultBuilder = new TransientFactory<IBuilder, InterfaceBuilder>();
            Interception = new List<InterceptionQuery>();
            _types = new Dictionary<string, ContainerType>();
            _lockResolve = new Object();
            _containerTypesIndex = 0;
        }

        public void Dispose()
        {
        }

        public T Resolve<T>()
        {
            var type = typeof(T);
            return (T)Resolve(type);
        }

        public object Resolve(Type type)
        {
            var name = type.FullName;

            lock (_lockResolve)
            {
                if (!_types.ContainsKey(name))
                {
                    var newType = ResolveForType(type);

                    _types.Add(name, newType);
                }
            }
            return _types[name].Lifetime.GetInstance(_types[name]);
        }

        private ContainerType ResolveForType(Type type)
        {
            var context = new ResolverContext
            {
                Container = this,
                ToResolveType = type
            };

            Resolvers.Run(context);

            if (context.ResolvedType == null)
            {
                throw new Exception("Resolver for type x not found!");
            }

            var lifeTimeFactory = context.ResolvedLifetime ?? DefaultLifetime;
            var builderFactory = context.ResolvedBuilder ?? DefaultBuilder;

            return new ContainerType
            {
                Index = _containerTypesIndex++,
                Container = this,
                ResolvedType = context.ResolvedType,
                Lifetime = lifeTimeFactory.GetInstance(),
                Builder = builderFactory.GetInstance()
            };
        }
    }
}