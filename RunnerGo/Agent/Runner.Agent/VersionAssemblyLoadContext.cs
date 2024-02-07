using System.Reflection;
using System.Runtime.Loader;

namespace Runner.Agent
{
    public  class VersionAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string _baseDirectory;

        public VersionAssemblyLoadContext(string baseDirectory)
            : base(true)
        {
            _baseDirectory = baseDirectory;
        }
        
        
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var name = assemblyName.Name ?? "";

            if (name.EndsWith("resources"))
            {
                return null;
            }

            var fullPath = Path.Combine(_baseDirectory, name + ".dll");
            if (File.Exists(fullPath))
            {
                return LoadFromAssemblyPath(fullPath);
            }
            return null;
        }
    }
}
