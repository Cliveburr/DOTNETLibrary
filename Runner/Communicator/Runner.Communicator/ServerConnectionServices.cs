using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runner.Communicator
{
    public class ServerConnectionServices
    {
        private object _target;

        public ServerConnectionServices(object target)
        {
            _target = target;
        }

        public object? Invoke(string? method, byte[]?[]? argsBytes)
        {
            if (method == null)
            {
                throw new Exception("Invalid Method");
            }


            var methodInfo = _target.GetType().GetMethod(method);
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

            return methodInfo.Invoke(_target, args)!;
        }
    }
}
