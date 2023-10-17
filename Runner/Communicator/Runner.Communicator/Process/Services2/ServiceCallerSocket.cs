using Microsoft.Extensions.DependencyInjection;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Runner.Communicator.Process.Services2
{
    public class ServiceCallerSocket : ServiceCallerBase
    {
        private readonly Client _client;

        public ServiceCallerSocket(Client client, IServiceScope? serviceScope, CancellationToken cancellationToken)
            : base(serviceScope, cancellationToken)
        {
            _client = client;
        }

        protected override async Task<InvokeResponse> SendAndReceive(InvokeRequest request)
        {
            var requestMessage = Message.Create(MessagePort.Services, request.GetBytes());
            var responseMessage = await _client.SendAndReceive(requestMessage);
            if (responseMessage.Head.Type != MessagePort.Services)
            {
                throw new Exception("Invalid response MessageType Services!");
            }
            var response = InvokeResponse.Parse(responseMessage.Data);
            return response;
        }

        public void Start()
        {
            Task.Run(StartAsync);
        }

        private async Task StartAsync()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = await _client.ReceiveMessage();

                    var response = await ProcessRequestAsync(message);
                    await _client.SendMessage(response);
                }
                catch (Exception err)
                {
                    _ = Task.Run(() => _client.OnError?.Invoke(this, err));
                }
            }
        }
    }
}
