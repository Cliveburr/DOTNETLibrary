using DependencyResolver.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyResolver.Extensions
{
    public static class ContainerExtensions
    {
        public static RegisterResolverHelper WithRegisterResolver(this Container container)
        {
            return new RegisterResolverHelper(container);
        }
    }
}