using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Loader;
using System.Reflection;

namespace Runner.Agent
{
    public class DynamicLoader
    {
        private AssemblyLoadContext? _context;
        private Assembly? _assembly;
        private object? _instance;
        private string[] _args;

        public DynamicLoader(string[] args)
        {
            _args = args;
        }

        private bool t;

        private string FoundAssemblyPath()
        {
            if (t)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", "v0", "Runner.Agent.Version.dll");
            }
            else
            {
                t = true;
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", "dev", "Runner.Agent.Version.dll");
            }
        }

        public void Run()
        {
            var assemblyPath = FoundAssemblyPath();

            _context = new AssemblyLoadContext("DynamicLoader", true);
            _assembly = _context.LoadFromAssemblyPath(assemblyPath);
            if (_assembly is null)
            {
                throw new Exception("Invalid assemblyPath! " + assemblyPath);
            }

            var type = _assembly.GetType("Runner.Agent.Version.Starter", false, true);
            if (type is null)
            {
                throw new Exception("Cannot find Start class!");
            }

            _instance = Activator.CreateInstance(type);
            if (_instance is null)
            {
                throw new Exception("Invalid instance! " + type.FullName);
            }

            var method = type.GetMethod("Run");
            if (method is null)
            {
                throw new Exception("Invalid Run method! " + type.FullName);
            }

            method.Invoke(_instance, new object[] { _args, (object)DesatachedReload, (object)DesatachedClose });
        }

        private void DesatachedReload()
        {
            Task.Run(Reload);
        }

        private void Reload()
        {
            Unload();
            Run();
        }

        private void DesatachedClose()
        {
            Task.Run(Close);
        }

        private void Close()
        {
            Unload();
            Environment.Exit(0);
        }

        private void Unload()
        {
            var instanceDisposable = _instance as IDisposable;
            if (instanceDisposable is not null)
            {
                instanceDisposable.Dispose();
            }
            _context?.Unload();
            //WaitUnload();
            _context = null;
            _instance = null;
            _assembly = null;
            GC.Collect(0, GCCollectionMode.Forced);
            GC.Collect(0, GCCollectionMode.Forced);
        }

        //private void WaitUnload()
        //{
        //    Task.Run(WaitUnloadAsync).Wait();
        //}

        //private async Task WaitUnloadAsync()
        //{
        //    var timeout = DateTime.Now.AddMilliseconds(6000 * 3);

        //    while (AssemblyIsLoaded() && DateTime.Now < timeout)
        //    {
        //        await Task.Delay(1000);
        //    }

        //    if (AssemblyIsLoaded())
        //    {
        //        var a = 1;
        //    }
        //}

        //private bool AssemblyIsLoaded()
        //{
        //    return AppDomain.CurrentDomain.GetAssemblies()
        //        .Any(a => a.FullName == (_assembly?.FullName ?? ""));

        //}
    }
}
