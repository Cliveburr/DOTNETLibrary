using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Hosting.Hubs
{
    public class AgentHub : Hub
    {
        public AgentHub()
        {
            
        }

        public Task Hit()
        {
            return Task.CompletedTask;
        }
    }
}
