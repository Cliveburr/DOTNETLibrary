
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
