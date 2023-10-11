using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Helpers
{
    public class ManuaResetEventAsync<T>
    {
        private ManualResetEvent _manualReset;
        private T? _value;

        public ManuaResetEventAsync()
        {
            _manualReset = new ManualResetEvent(false);
        }

        public void Set(T value)
        {
            _value = value;
            _manualReset.Set();
        }

        public Task<T> WaitAsync(int millisecondsTimeout = 1000)
        {
            return Task.Run(new Func<T>(() =>
            {
                if (millisecondsTimeout == 0)
                {
                    _manualReset.WaitOne();
                }
                else
                {
                    _manualReset.WaitOne(millisecondsTimeout);
                }

                if (_manualReset.WaitOne(millisecondsTimeout))
                {
                    return _value!;
                }
                else
                {
                    throw new TimeoutException();
                }
            }));
        }
    }
}
