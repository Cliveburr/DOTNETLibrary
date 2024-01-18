using Runner.WebUI.Components.Modal;
using System.ComponentModel;

namespace Runner.WebUI.Components.Notification
{
    public class NotificationService
    {
        public NotificationComponent? Component { get; set; }
        public List<NotificationData> Notifications = new List<NotificationData>();

        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task Add(NotificationData data, int timeoutSeconds = 0)
        {
            if (timeoutSeconds > 0)
            {
                data.Timer = new Timer(Timeout, data, 0, timeoutSeconds * 1000);
            }
            lock (Notifications)
            {
                Notifications.Add(data);
            }
            if (Component != null)
            {
                await Component.InvokeStateHasChangedAsync();
            }
        }

        private void Timeout(object? state)
        {
            var not = state as NotificationData;
            if (not != null)
            {
                not.Timer?.Dispose();
                lock (Notifications)
                {
                    if (Notifications.Contains(not))
                    {
                        Notifications.Remove(not);
                        if (Component != null)
                        {
                            Component.InvokeStateHasChangedAsync().Wait();
                        }
                    }
                }
            }
        }

        public void Remove(NotificationData not)
        {
            lock (Notifications)
            {
                not.Timer?.Dispose();
                if (Notifications.Contains(not))
                {
                    Notifications.Remove(not);
                    if (Component != null)
                    {
                        Component.InvokeStateHasChangedAsync().Wait();
                    }
                }
            }
        }

        public Task AddInfo(string text, int timeoutSeconds = 0)
        {
            Assert.MustNotNull(Component, "Notification is not ready!");
            return Add(new NotificationData
            {
                Class = "is-info",
                Text = text
            }, timeoutSeconds);
        }

        public Task AddError(Exception exception)
        {
            Assert.MustNotNull(Component, "Notification is not ready!");

            if (!exception.GetType().Equals(typeof(RunnerException)))
            {
                _logger.LogError(exception.ToString());
            }

            return Add(new NotificationData
            {
                Class = "is-danger",
                Text = exception.Message
            });
        }

        public Task AddError(string text)
        {
            Assert.MustNotNull(Component, "Notification is not ready!");

            return Add(new NotificationData
            {
                Class = "is-danger",
                Text = text
            });
        }
    }
}
