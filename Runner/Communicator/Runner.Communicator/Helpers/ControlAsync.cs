using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Helpers
{
    public class ControlAsync
    {
        private Queue<ControlItem> _items;
        private object _lock = new object();
        private bool _hasItem;

        private class ControlItem
        {
            public ManualResetEvent Event  { get; } = new ManualResetEvent(false);
            public Task? Task { get; set; }
        }

        public ControlAsync()
        {
            _items = new Queue<ControlItem>();
            _lock = new object();
            _hasItem = false;
        }

        public Task CheckToPass()
        {
            lock (_lock)
            {
                if (_hasItem)
                {
                    var newItem = new ControlItem();
                    newItem.Task = Task.Run(() =>
                    {
                        newItem.Event.WaitOne();
                    });
                    _items.Enqueue(newItem);
                    return newItem.Task;
                }
                else
                {
                    _hasItem = true;
                    return Task.CompletedTask;
                }
            }
        }

        public void ReleasePass()
        {
            lock (_lock)
            {
                if (_items.Count == 0)
                {
                    _hasItem = false;
                }
                else
                {
                    var item = _items.Dequeue();
                    item.Event.Set();
                }
            }
        }
    }
}
