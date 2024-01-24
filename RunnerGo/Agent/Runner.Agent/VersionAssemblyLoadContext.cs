using System.Reflection;
using System.Runtime.Loader;

namespace Runner.Agent
{
    public  class VersionAssemblyLoadContext : AssemblyLoadContext
    {
        public VersionAssemblyLoadContext()
            : base(true)
        {
        }
        
        protected override Assembly? Load(AssemblyName name)
        {
            return null;
        }
    }
}
