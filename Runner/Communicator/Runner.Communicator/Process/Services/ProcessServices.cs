using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Runner.Communicator.Model;

namespace Runner.Communicator.Process.Services
{
    public class ProcessServices
    {
        private Server _server;
        private Dictionary<string, ServerProxy> _services;
        private IServiceScope? _serviceScope;

        public ProcessServices(Server server)
        {
            _server = server;
            _services = new Dictionary<string, ServerProxy>();
        }

        public async Task<Message> ProcessRequest(Message message)
        {
            try
            {
                var request = RequestModel.Parse(message.Data);

                var interfaceFullName = request.InterfaceFullName;
                if (interfaceFullName == null)
                {
                    throw new Exception("Invalid InterfaceFullName");
                }

                if (!_services.ContainsKey(interfaceFullName))
                {
                    var serverService = _server.Services.GetForType(interfaceFullName);
                    if (serverService == null)
                    {
                        throw new Exception("Interface invalid: " + interfaceFullName);
                    }

                    var target = Build(serverService.Interface);
                    var newService = _services[interfaceFullName] = new ServerProxy(target);
                }
                var service = _services[interfaceFullName];

                var resultAwaitable = service.Invoke(request.Method, request.Args)!;
                var resultType = resultAwaitable.GetType();
                var resultTask = (Task)resultAwaitable;
                if (resultType.Name == "DelayPromise")
                {
                    await resultTask.WaitAsync(_server.CancellationToken);
                }
                //else if (resultType.Name.StartsWith("Task"))
                //{
                //    if (!resultTask.IsCompleted)
                //    {
                //        await resultTask.WaitAsync(_server.CancellationToken);
                //    }
                //}
                else
                {
                    if (!resultTask.IsCompleted)
                    {
                        await resultTask.WaitAsync(_server.CancellationToken);
                    }
                    //throw new Exception("Unsupported Task return! " + resultType.FullName);
                }

                var get_Result = resultType.GetMethod("get_Result");
                var result = get_Result?.Invoke(resultAwaitable, null);

                var resultJson = JsonSerializer.Serialize(result);
                var resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                var response = new ResponseModel
                {
                    IsSuccess = true,
                    Result = resultBuffer
                };

                return Message.Create(MessageType.Services, response.GetBytes());
            }
            catch (TargetInvocationException err)
            {
                var response = new ResponseModel
                {
                    IsSuccess = false,
                    Result = Encoding.UTF8.GetBytes(err.InnerException!.ToString())
                };

                return Message.Create(MessageType.Services, response.GetBytes());
            }
            catch (Exception err)
            {
                var response = new ResponseModel
                {
                    IsSuccess = false,
                    Result = Encoding.UTF8.GetBytes(err.ToString())
                };

                return Message.Create(MessageType.Services, response.GetBytes());
            }
        }

        private object Build(Type type)
        {
            if (_serviceScope == null)
            {
                //var serviceProvider = _server.ServiceCollection.BuildServiceProvider();
                _serviceScope = _server.Services.GetScope(); // serviceProvider.CreateScope();
            }
            return _serviceScope.ServiceProvider.GetRequiredService(type);
        }
    }
}
