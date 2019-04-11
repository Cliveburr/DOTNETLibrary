using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Factory
{
    public class FactoryAttribute : Attribute
    {
        public IFactory GetFactory { get; }
    }
}