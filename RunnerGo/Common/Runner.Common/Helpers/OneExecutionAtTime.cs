
namespace Runner.Common.Helpers
{
    public class OneExecutionAtTime
    {
        private readonly Func<Task> _func;
        private bool _isRunning;
        private bool _flagToRun;
        private int _interval;
        private object _lock = new object();

        public OneExecutionAtTime(Func<Task> func, int interval = 300)
        {
            _func = func;
            _interval = interval;
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    _flagToRun = true;
                }
                else
                {
                    _isRunning = true;
                    _ = Task.Run(InnerRun);
                }
            }
        }

        private async Task InnerRun()
        {
            try
            {
                await _func();
            }
            catch { }

            lock (_lock)
            {
                if (_flagToRun)
                {
                    _flagToRun = false;
                    _ = Task.Run(DelayToRun);
                }
                else
                {
                    _isRunning = false;
                }
            }
        }

        private async Task DelayToRun()
        {
            await Task.Delay(_interval);
            await InnerRun();
        }
    }
}
