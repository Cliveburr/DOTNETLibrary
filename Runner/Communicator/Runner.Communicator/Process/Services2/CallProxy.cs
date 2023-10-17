using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services2
{
    public class CallProxy<T> : DispatchProxy
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Func<InvokeRequest, Task<InvokeResponse>> _callInvoker;
        private string _interfaceTypeQualifiedName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static T Create(Func<InvokeRequest, Task<InvokeResponse>> callInvoker, string interfaceTypeQualifiedName)
        {
            object proxy = Create<T, CallProxy<T>>()!;
            ((CallProxy<T>)proxy).Initialize(callInvoker, interfaceTypeQualifiedName);
            return (T)proxy;
        }

        public void Initialize(Func<InvokeRequest, Task<InvokeResponse>> callInvoker, string interfaceTypeQualifiedName)
        {
            _callInvoker = callInvoker;
            _interfaceTypeQualifiedName = interfaceTypeQualifiedName;
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
                var invokeDirectAsyncGeneric = typeof(CallProxy<T>)
                    .GetMethod("InvokeDirectAsync", BindingFlags.Instance | BindingFlags.Public)!;

                return invokeDirectAsyncGeneric.Invoke(this, new object?[] { targetMethod.Name, args });
            }
            else
            {
                var invokeAsyncGeneric = typeof(CallProxy<T>)
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

            var request = new InvokeRequest
            {
                AssemblyQualifiedName = _interfaceTypeQualifiedName,
                Method = method,
                Args = argsBytes
            };
            var response = await _callInvoker(request);

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

            var request = new InvokeRequest
            {
                AssemblyQualifiedName = _interfaceTypeQualifiedName,
                Method = method,
                Args = argsBytes
            };
            var response = await _callInvoker(request);

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
