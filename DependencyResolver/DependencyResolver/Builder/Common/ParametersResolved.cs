using DependencyResolver.Resolvers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Builder.Common
{
    public class ParametersResolved
    {
        public ParameterInfo Parameter { get; set; }
        public ResolvedType ResolvedType { get; set; }
        public object DefaultValue { get; set; }
    }
}