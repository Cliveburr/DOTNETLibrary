using System;
using System.Collections.Generic;
using System.Text;
using LearnAOP.AOP.Resolver;

namespace LearnAOP.AOP.Lifetime
{
    public class TransientLifetime : ILifetime
    {
        public object GetInstance(ContainerType containerType)
        {
            return containerType.Builder.Generate(containerType);
        }
    }
}