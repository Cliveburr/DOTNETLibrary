using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Containers
{
    public class ResolveContext
    {
        public IContainer Container { get; set; }
        public ChainDependency Chain { get; set; }
        public Dictionary<string, object> Bag { get; set; }
    }
}