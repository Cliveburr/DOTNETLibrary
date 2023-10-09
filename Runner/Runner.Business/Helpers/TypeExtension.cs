using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Helpers
{
    public static class TypeExtension
    {
        public static IEnumerable<Type> GetAllAssignableFrom(this Type taskBaseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => taskBaseType.IsAssignableFrom(t) && !t.Equals(taskBaseType));
        }
    }
}
