using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Helpers
{
    public static class AttributeHelper
    {
        public static IEnumerable<T> GetAttributes<T>(params Type[] types) where T : Attribute
        {
            return types
                .SelectMany(t => t.GetCustomAttributes(typeof(T), true))
                .OfType<T>();
        }

        public static T GetFirstAttribute<T>(Type fromType) where T : Attribute
        {
            var attribs = fromType.GetCustomAttributes(typeof(T), true);

            if (attribs.Length > 0)
                return (T)attribs[0];
            else
                return default(T);
        }

        public static T GetFirstAttribute<T>(params Type[] priorityOptionType) where T : Attribute
        {
            foreach (var optionType in priorityOptionType)
            {
                var attrib = GetFirstAttribute<T>(optionType);

                if (attrib != null)
                    return attrib;
            }
            return default(T);
        }

        public static IEnumerable<T> GetAttributes<T>(MethodInfo methodInfo) where T : Attribute
        {
            return methodInfo.GetCustomAttributes<T>(true);
        }
    }
}