using System.Reflection;

namespace Runner.Kernel.Helpers
{
    public static class TypeExtension
    {
        public static IEnumerable<Type> GetAllAssignableFrom(this Type taskBaseType, Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => taskBaseType.IsAssignableFrom(t) && !t.Equals(taskBaseType));
        }

        public static IEnumerable<Type> GetAllFromAbstract(this Type abstractType, Assembly assembly)
        {
            //return assembly.GetTypes()
            //    .Where(t => t.IsClass && !t.IsAbstract && taskBaseType.IsAssignableFrom(t) && !t.IsInheritedFrom(taskBaseType));
            return from x in assembly.GetTypes()
                   let y = x.BaseType
                   where
                      (y != null && y.IsGenericType && abstractType.IsAssignableFrom(y.GetGenericTypeDefinition()))
                   select x;
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(this Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                       (y != null && y.IsGenericType && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                       (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }
    }
}
