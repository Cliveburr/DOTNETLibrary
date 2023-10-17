using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Process.Services2
{
    public class ServiceCallerBackObj
    {
        public object? Service { get; set; }
    }

    public class ServiceCallerBack<T> where T : ServiceCallerBase
    {
        public T Service { get => (T)_serviceCallerBackObj.Service!; }

        private ServiceCallerBackObj _serviceCallerBackObj;

        public ServiceCallerBack(ServiceCallerBackObj serviceCallerBackObj)
            : base()
        {
            _serviceCallerBackObj = serviceCallerBackObj;
        }
    }
}
