using Microsoft.AspNetCore.SignalR.Client;

namespace Runner.Agent.Version
{
    public class KeepAlwaysConnected : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(3);
        }
    }
}
