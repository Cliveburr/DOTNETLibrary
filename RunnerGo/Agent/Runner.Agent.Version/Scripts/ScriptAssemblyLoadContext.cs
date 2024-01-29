using System.Reflection;
using System.Runtime.Loader;

namespace Runner.Agent.Version.Scripts
{
    public class ScriptAssemblyLoadContext : AssemblyLoadContext
    {
        public ScriptAssemblyLoadContext()
            : base(true)
        {
        }

        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }
    }
}
