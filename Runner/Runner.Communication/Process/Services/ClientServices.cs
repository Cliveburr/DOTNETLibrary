using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communication.Process.Services
{
    public class ClientServices
    {
        private Dictionary<string, object> _proxys;

        public ClientServices()
        {
            _proxys = new Dictionary<string, object>();
        }

        public T Open<T>(Client client)
        {
            var interfaceType = typeof(T);
            var interfaceTypeFullName = interfaceType.FullName!;

            if (!_proxys.ContainsKey(interfaceTypeFullName))
            {
                var proxyType = typeof(ClientProxy<>);
                var proxyGenericType = proxyType.MakeGenericType(interfaceType);

                _proxys[interfaceTypeFullName] = proxyGenericType.InvokeMember("Create", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { client, interfaceTypeFullName })!;
            }
            return (T)_proxys[interfaceTypeFullName];
        }
    }
}
