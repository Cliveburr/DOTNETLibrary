using Runner.Communicator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services
{
    public class ClientProxy<T> : DispatchProxy
    {
        private Client? _client;
        private string? _interfaceTypeFullName;

        public static T Create(Client client, string interfaceTypeFullName)
        {
            object proxy = Create<T, ClientProxy<T>>()!;
            ((ClientProxy<T>)proxy).Initialize(client, interfaceTypeFullName);
            return (T)proxy;
        }

        public void Initialize(Client client, string interfaceTypeFullName)
        {
            _client = client;
            _interfaceTypeFullName = interfaceTypeFullName;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null)
            {
                throw new NullReferenceException("MethodInfo");
            }

            Type? returnType = null;
            if (targetMethod.ReturnType.Namespace == "System.Threading.Tasks")
            {
                if (targetMethod.ReturnType.GenericTypeArguments.Length > 0)
                {
                    returnType = targetMethod.ReturnType.GenericTypeArguments[0];
                }
            }
            else
            {
                throw new Exception($"Method missing Task return: \"{targetMethod.Name}\"!");
            }

            if (returnType == null)
            {
                var invokeDirectAsyncGeneric = typeof(ClientProxy<T>)
                    .GetMethod("InvokeDirectAsync", BindingFlags.Instance | BindingFlags.Public)!;

                return invokeDirectAsyncGeneric.Invoke(this, new object?[] { targetMethod.Name, args });
            }
            else
            {
                var invokeAsyncGeneric = typeof(ClientProxy<T>)
                    .GetMethod("InvokeAsync", BindingFlags.Instance | BindingFlags.Public)!;

                var genericMethod = invokeAsyncGeneric.MakeGenericMethod(
                    new Type[] { returnType });

                return genericMethod.Invoke(this, new object?[] { targetMethod.Name, args });
            }
        }

        public async Task<D?> InvokeAsync<D>(string method, object[] args)
        {
            var argsBytes = args
                .Select(a =>
                {
                    var argJson = JsonSerializer.Serialize(a);
                    return Encoding.UTF8.GetBytes(argJson);
                })
                .ToArray();

            var request = new RequestModel
            {
                InterfaceFullName = _interfaceTypeFullName!,
                Method = method,
                Args = argsBytes
            };
            var requestMessage = Message.Create(MessagePort.Services, request.GetBytes());

            var responseMessage = await _client!.SendAndReceive(requestMessage);
            if (responseMessage.Head.Type != MessagePort.Services)
            {
                throw new Exception("Invalid response MessageType Services!");
            }
            var response = ResponseModel.Parse(responseMessage.Data);

            if (response.IsSuccess)
            {
                if (response.Result == null)
                {
                    return default;
                }
                else
                {
                    var resultJson = Encoding.UTF8.GetString(response.Result);
                    return JsonSerializer.Deserialize<D>(resultJson);
                }
            }
            else
            {
                if (response.Result == null)
                {
                    throw new Exception();
                }
                else
                {
                    var resultJson = Encoding.UTF8.GetString(response.Result);
                    throw new Exception(resultJson);
                }
            }
        }

        public async Task InvokeDirectAsync(string method, object[] args)
        {
            var argsBytes = args
                .Select(a =>
                {
                    var argJson = JsonSerializer.Serialize(a);
                    return Encoding.UTF8.GetBytes(argJson);
                })
                .ToArray();

            var request = new RequestModel
            {
                InterfaceFullName = _interfaceTypeFullName!,
                Method = method,
                Args = argsBytes
            };
            var requestMessage = Message.Create(MessagePort.Services, request.GetBytes());

            var responseMessage = await _client!.SendAndReceive(requestMessage);
            if (responseMessage.Head.Type != MessagePort.Services)
            {
                throw new Exception("Invalid response MessageType!");
            }
            var response = ResponseModel.Parse(responseMessage.Data);

            if (response.IsSuccess)
            {
                return;
            }
            else
            {
                if (response.Result == null)
                {
                    throw new Exception();
                }
                else
                {
                    var resultJson = Encoding.UTF8.GetString(response.Result);
                    throw new Exception(resultJson);
                }
            }
        }
    }
}
