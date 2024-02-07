using System.Reflection;
using System.Runtime.Loader;

namespace Runner.Agent.Version.Scripts
{
    public class ScriptAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string _baseDirectory;
        private readonly AssemblyLoadContext _agentVersionContext;

        public ScriptAssemblyLoadContext(string baseDirectory, AssemblyLoadContext agentVersionContext)
            : base(true)
        {
            _baseDirectory = baseDirectory;
            _agentVersionContext = agentVersionContext;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assembly = _agentVersionContext.LoadFromAssemblyName(assemblyName);
            if (assembly is not null)
            {
                return assembly;
            }

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
