using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Process.Services2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Services
{
    public class ServiceCallerImp : ServiceCallerBase
    {
        public Func<InvokeRequest, Task<InvokeResponse>>? ToCallInoker { get; set; }

        public ServiceCallerImp(IServiceScope serviceScope, CancellationToken cancellationToken)
            : base(serviceScope, cancellationToken)
        {
        }

        protected override Task<InvokeResponse> SendAndReceive(InvokeRequest request)
        {
            if (ToCallInoker == null)
            {
                throw new Exception();
            }
            return ToCallInoker(request);
        }

        public Task<InvokeResponse> CallInvokeAsync(InvokeRequest request)
        {
            return base.InvokeAsync(request);
        }
    }
}
