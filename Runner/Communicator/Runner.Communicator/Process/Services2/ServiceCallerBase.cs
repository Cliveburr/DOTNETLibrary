using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services2
{
    public abstract class ServiceCallerBase : IDisposable
    {
        private IServiceScope _serviceScope;
        private CancellationTokenSource _cancellationTokenSource;

        public ServiceCallerBase(IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            _serviceScope = serviceScope;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        protected abstract Task<InvokeResponse> SendAndReceive(InvokeRequest request);

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public T Open<T>() where T : class
        {
            var interfaceType = typeof(T);
            var assemblyQualifiedName = interfaceType.AssemblyQualifiedName!;

            var proxyType = typeof(CallProxy<>);
            var proxyGenericType = proxyType.MakeGenericType(interfaceType);

            var proxy = proxyGenericType.InvokeMember("Create", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { SendAndReceive, assemblyQualifiedName })!;
            return (T)proxy;
        }

        protected virtual async Task<InvokeResponse> InvokeAsync(InvokeRequest request)
        {
            try
            {
                var interfaceFullName = request.AssemblyQualifiedName;
                if (interfaceFullName == null)
                {
                    throw new Exception("Invalid InterfaceFullName");
                }

                var serviceType = Type.GetType(interfaceFullName);
                if (serviceType == null)
                {
                    throw new Exception("Interface invalid: " + interfaceFullName);
                }

                var target = _serviceScope.ServiceProvider.GetRequiredService(serviceType);

                var resultAwaitable = TargetInvoke(target, request.Method, request.Args)!;
                var resultType = resultAwaitable.GetType();
                var resultTask = (Task)resultAwaitable;
                if (resultType.Name == "DelayPromise")
                {
                    await resultTask.WaitAsync(_cancellationTokenSource.Token);
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
                        await resultTask.WaitAsync(_cancellationTokenSource.Token);
                    }
                    //throw new Exception("Unsupported Task return! " + resultType.FullName);
                }

                var get_Result = resultType.GetMethod("get_Result");
                var result = get_Result?.Invoke(resultAwaitable, null);

                var resultJson = JsonSerializer.Serialize(result);
                var resultBuffer = Encoding.UTF8.GetBytes(resultJson);

                return new InvokeResponse
                {
                    IsSuccess = true,
                    Result = resultBuffer
                };
            }
            catch (TargetInvocationException err)
            {
                return new InvokeResponse
                {
                    IsSuccess = false,
                    Result = Encoding.UTF8.GetBytes(err.InnerException!.ToString())
                };
            }
            catch (Exception err)
            {
                return new InvokeResponse
                {
                    IsSuccess = false,
                    Result = Encoding.UTF8.GetBytes(err.ToString())
                };
            }
        }

        private object? TargetInvoke(object target, string? method, byte[]?[]? argsBytes)
        {
            if (method == null)
            {
                throw new Exception("Invalid Method");
            }

            var methodInfo = target.GetType().GetMethod(method);
            if (methodInfo == null)
            {
                throw new Exception("Invalid method! " + method);
            }

            object?[]? args = null;
            if (argsBytes != null)
            {
                if (argsBytes.Length == 0)
                {
                    args = new object?[0];
                }
                else
                {
                    var paras = methodInfo.GetParameters();
                    args = new object?[argsBytes.Length];
                    for (var i = 0; i < argsBytes.Length; i++)
                    {
                        var type = paras[i].ParameterType;
                        var bytes = argsBytes[i];
                        if (bytes != null)
                        {
                            var obj = JsonSerializer.Deserialize(Encoding.UTF8.GetString(bytes), type);
                            args[i] = obj;
                        }
                    }
                }
            }

            return methodInfo.Invoke(target, args)!;
        }
    }
}
