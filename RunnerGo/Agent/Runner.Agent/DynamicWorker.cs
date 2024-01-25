using Runner.Agent.vers;
using System.Runtime.CompilerServices;

namespace Runner.Agent
{
    public class DynamicWorker : IHostedService, IDisposable
    {
        private readonly ILogger<DynamicWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _stoppingSource;
        private bool _shouldReload;
        private bool _shouldDowngrade;

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
            if (_shouldDowngrade)
            {
                try
                {
                    VersionInfo.PeformADownGrade();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                }
                _shouldDowngrade = false;
            }

            var assemblyPath = FoundAssemblyPath();

            _stoppingSource = new CancellationTokenSource();

            var context = new VersionAssemblyLoadContext();
            WeakReference contextRef;
            ExecuteAndUnload(out contextRef);

            for (int i = 0; contextRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            var timeout = DateTime.Now.AddMilliseconds(60000);
            while (contextRef.IsAlive && AssemblyIsLoaded() && DateTime.Now < timeout)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Task.Delay(100).Wait();
            }

            if (AssemblyIsLoaded() || contextRef.IsAlive)
            {
                //throw new Exception("AssemblyLoadContext not unloaded!");
                _logger.LogWarning("AssemblyLoadContext not unloaded!");
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
            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                _shouldReload = true;
                _shouldDowngrade = true;
            }
            finally
            {
                context.Unload();
            }
        }

        private bool AssemblyIsLoaded()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => (a.FullName ?? "").Contains("Runner.Agent.Version"));

        }
    }
}
