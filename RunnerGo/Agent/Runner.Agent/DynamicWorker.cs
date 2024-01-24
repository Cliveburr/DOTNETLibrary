using Runner.Agent.vers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Runner.Agent
{
    public class DynamicWorker : IHostedService, IDisposable
    {
        private readonly ILogger<DynamicWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        //private VersionAssemblyLoadContext? _context;
        //private WeakReference? _contextRef;
        //private Assembly? _assembly;
        //private Type? _type;
        //private object? _instance;
        private CancellationTokenSource? _stoppingSource;
        private bool _shouldReload;

        public DynamicWorker(ILogger<DynamicWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started dynamic worker at: {time}", DateTimeOffset.Now);

            StartVersion();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped dynamic worker at: {time}", DateTimeOffset.Now);

            if (_stoppingSource is not null && !_stoppingSource.IsCancellationRequested)
            {
                _stoppingSource.Cancel();
            }

            return Task.CompletedTask;
        }

        private string FoundAssemblyPath()
        {
            var actualVersion = VersionInfo.ReadVersionActual();

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", actualVersion, "Runner.Agent.Version.dll");
        }

        public void StartVersion()
        {
            Task.Run(StartVersionWrap);
        }

        private void StartVersionWrap()
        {
            var assemblyPath = FoundAssemblyPath();

            _stoppingSource = new CancellationTokenSource();

            var context = new VersionAssemblyLoadContext();
            WeakReference contextRef;
            ExecuteAndUnload(out contextRef);

            //context.Unload();
            //context = null;
            ////_instance = null;
            //var fullName = assembly?.FullName ?? "";
            //assembly = null;
            ////GC.Collect();
            ////GC.Collect();

            //WaitUnloadAsync(fullName, contextRef).Wait();

            for (int i = 0; contextRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            var timeout = DateTime.Now.AddMilliseconds(6000 * 3);
            while (contextRef.IsAlive && AssemblyIsLoaded() && DateTime.Now < timeout)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Task.Delay(100).Wait();
            }

            if (AssemblyIsLoaded() || contextRef.IsAlive)
            {
                throw new Exception("AssemblyLoadContext not unloaded!");
            }

            if (_shouldReload)
            {
                _shouldReload = false;
                Task.Run(StartVersionWrap);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteAndUnload(out WeakReference contextRef)
        {
            var assemblyPath = FoundAssemblyPath();

            var context = new VersionAssemblyLoadContext();
            contextRef = new WeakReference(context, true);
            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            if (assembly is null)
            {
                throw new Exception("Invalid assemblyPath! " + assemblyPath);
            }

            var type = assembly.GetType("Runner.Agent.Version.WorkerEntry", false, true);
            if (type is null)
            {
                throw new Exception("Cannot find WorkerEntry class!");
            }

            var executeMethod = type.GetMethod("Execute");
            if (executeMethod is null)
            {
                throw new Exception("Invalid Start method! " + type.FullName);
            }

            var result = (bool?)executeMethod.Invoke(null, new object[] { _serviceProvider, _stoppingSource!.Token });
            _shouldReload = result ?? false;

            context.Unload();
        }

        public void StopVersion()
        {
            //if (_type is not null /*&& _instance is not null*/)
            //{
            //    var method = _type.GetMethod("Stop");
            //    if (method is null)
            //    {
            //        throw new Exception("Invalid Stop method! " + _type.FullName);
            //    }

            //    method.Invoke(null, null);
            //}

            ////var instanceDisposable = _instance as IDisposable;
            ////if (instanceDisposable is not null)
            ////{
            ////    instanceDisposable.Dispose();
            ////}

            //_context?.Unload();
            //_context = null;
            ////_instance = null;
            //var fullName = _assembly?.FullName ?? "";
            //_assembly = null;
            //GC.Collect();
            //GC.Collect();

            //WaitUnloadAsync(fullName).Wait();
        }

        private void DesatachedReload()
        {
            Task.Run(Reload);
        }

        private void Reload()
        {
            //Unload();
            StartVersion();
        }

        private void DesatachedClose()
        {
            Task.Run(Close);
        }

        private void Close()
        {
            //Unload();
            Environment.Exit(0);
        }

        //private async Task WaitUnloadAsync(string fullName, WeakReference contextRef)
        //{
        //    var timeout = DateTime.Now.AddMilliseconds(6000 * 3);

        //    while (AssemblyIsLoaded(fullName) && DateTime.Now < timeout)
        //    {
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();

        //        await Task.Delay(1000);
        //    }

        //    if (AssemblyIsLoaded(fullName))
        //    {
        //        var a = contextRef.IsAlive;
        //    }
        //}

        private bool AssemblyIsLoaded()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => (a.FullName ?? "").Contains("Runner.Agent.Version"));

        }
    }
}
