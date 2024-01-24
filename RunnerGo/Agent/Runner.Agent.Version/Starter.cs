//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System.Runtime.InteropServices.JavaScript;

//namespace Runner.Agent.Version
//{
//    public class Starter : IDisposable
//    {
//        private static Starter? _instance;
//        public static Starter Instance
//        {
//            get
//            {
//                if (_instance is null)
//                {
//                    throw new ArgumentNullException("Starter instance invalid!");
//                }
//                return _instance;
//            }
//        }

//        private IHost? _host;
//        private Action? _reload;
//        private Action? _close;

//        public Starter()
//        {
//            if (_instance is not null)
//            {
//                throw new InvalidOperationException("Only one instance of Starter is allowed!");
//            }
//            _instance = this;
//        }

//        public void Run(string[] args, Action reload, Action close)
//        {
//            _reload = reload;
//            _close = close;

//            Console.WriteLine("Start run");

//            //var hostBuilder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
//            //{
//            //});

//            //hostBuilder.Services.Configure<HostOptions>(c =>
//            //{
//            //    c.StartupTimeout = TimeSpan.FromMilliseconds(100);
//            //    c.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
//            //});
//            //hostBuilder.Services.AddHostedService<Worker>();

//            //_host = hostBuilder.Build();
//            //_host.Run();


//            _host = Host.CreateDefaultBuilder(args)
//                .ConfigureServices(services =>
//                {
//                    services.AddHostedService<Worker>();
//                })
//                .Build();

//            _host.Run();
//        }

//        private void Terminate()
//        {
//            _host?.Dispose();
//            _host = null;
//            GC.Collect(0, GCCollectionMode.Forced);
//            GC.Collect(0, GCCollectionMode.Forced);
//        }

//        public void Reload()
//        {
//            Terminate();
//            _reload?.Invoke();
//        }

//        public void Close()
//        {
//            _host?.StopAsync().Wait();
//            Terminate();
//            _close?.Invoke();
//        }

//        public void Dispose()
//        {
//            Terminate();
//            _instance = null;
//            _reload = null;
//            _close = null;
//        }
//    }
//}
