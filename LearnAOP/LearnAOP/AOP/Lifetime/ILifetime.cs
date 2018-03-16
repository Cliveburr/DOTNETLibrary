using LearnAOP.AOP.Resolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP.Lifetime
{
    public interface ILifetime
    {
        object GetInstance(ContainerType containerType);
    }
}