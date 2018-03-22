using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Helpers
{
    public static class AttributeHelper
    {
        public static T GetOneAttribute<T>(Type fromType) where T : Attribute
        {
            var attribs = fromType.GetCustomAttributes(typeof(T), true);

            if (attribs.Length > 1)
                throw new Exception($"The type \"{fromType.FullName }\" can have only one attribute of type \"{typeof(T).FullName}\"!");

            if (attribs.Length == 1)
                return (T)attribs[0];
            else
                return default(T);
        }

        public static T GetOneFromFirstAttribute<T>(params Type[] priorityOptionType) where T : Attribute
        {
            foreach (var optionType in priorityOptionType)
            {
                var attrib = GetOneAttribute<T>(optionType);

                if (attrib != null)
                    return attrib;
            }
            return default(T);
        }

        public static List<T> GetAttributes<T>(MethodInfo methodInfo) where T: Attribute
        {
            return methodInfo.GetCustomAttributes<T>(true)
                .ToList();
        }
    }
}