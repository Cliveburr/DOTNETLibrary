using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests
{
    public class HostService
    {
        public ServiceCollection? Services { get; private set; }
        public ServiceProvider? Provider { get; private set; }
        private Action<IServiceCollection>? _configureServices;

        public HostService ConfigureServices(Action<IServiceCollection> configureDelegate)
        {
            _configureServices = configureDelegate;
            return this;
        }

        public HostService Build()
        {
            if (_configureServices == null)
            {
                throw new NullReferenceException("ConfigureServices must be invoked!");
            }
            Services = new ServiceCollection();
            _configureServices(Services);
            Provider = Services.BuildServiceProvider();
            return this;
        }
    }
}
