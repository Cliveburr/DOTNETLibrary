using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Builder
{
    public class BuilderAttribute : Attribute
    {
        public IBuilder GetBuilder { get; }
    }
}