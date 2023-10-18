//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Runner.Communicator.Process.Services
//{
//    public class ServerServices
//    {
//        private List<ServicesType> _services;
//        private IServiceCollection _serviceCollection;

//        public class ServicesType
//        {
//            public required Type Interface { get; set; }
//            public required Type Implementation { get; set; }
//        }

//        public ServerServices(IServiceCollection serviceCollection)
//        {
//            _services = new List<ServicesType>();
//            _serviceCollection = serviceCollection;
//        }

//        public ServerServices Add<I, S>()
//            where I : class
//            where S : class, I
//        {
//            _serviceCollection
//                .AddScoped<I, S>();
//            _services.Add(new ServicesType
//            {
//                Interface = typeof(I),
//                Implementation = typeof(S)
//            });
//            return this;
//        }

//        internal ServicesType? GetForType(string interfaceFullName)
//        {
//            return _services
//                .FirstOrDefault(s => s.Interface.FullName == interfaceFullName);
//        }

//        internal IServiceScope GetScope()
//        {
//            var serviceProvider = _serviceCollection.BuildServiceProvider();
//            return serviceProvider.CreateScope();
//        }
//    }
//}
