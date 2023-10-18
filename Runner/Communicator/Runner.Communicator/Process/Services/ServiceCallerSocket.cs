using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Abstract;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services
{
    public class ServiceCallerSocket : ServiceCallerBase
    {
        private readonly SocketTcp _socketTcp;

        public ServiceCallerSocket(SocketTcp socketTcp, IServiceScope? serviceScope, CancellationToken cancellationToken)
            : base(serviceScope, cancellationToken)
        {
            _socketTcp = socketTcp;
        }

        protected override async Task<InvokeResponse> SendAndReceive(InvokeRequest request)
        {
            var data = await _socketTcp.SendAndReceive(request.GetBytes(), MessagePort.Services);
            var response = InvokeResponse.Parse(data);
            return response;
        }

        public async Task<byte[]> Process(byte[] data)
        {
            var request = InvokeRequest.Parse(data);
            var response = await base.InvokeAsync(request);
            return response.GetBytes();
        }
    }
}
