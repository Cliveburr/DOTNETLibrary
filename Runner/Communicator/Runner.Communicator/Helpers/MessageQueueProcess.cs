using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Helpers
{
    public class MessageQueueProcess<T>
    {
        private readonly Queue<T> _queue;
        private Func<T, Task> _process;
        private Task? _running;
        private object _lock = new object();

        public MessageQueueProcess(Func<T, Task> process)
        {
            _queue = new Queue<T>();
            _process = process;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
            }
            CheckAndRun();
        }

        public void CheckAndRun()
        {
            if (_running == null)
            {
                lock (_lock)
                {
                    if (_running == null)
                    {
                        _running = Task.Run(RunAsync);
                    }
                }
            }
        }

        private T? GetNextItem()
        {
            if (_queue.Count > 0)
            {
                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        return _queue.Dequeue();
                    }
                }
            }
            return default(T);
        }

        private async Task RunAsync()
        {
            var item = GetNextItem();
            while (item != null)
            {
                await _process(item);
                item = GetNextItem();
            }
            lock (_lock)
            {
                _running = null;
            }
        }
    }
}
