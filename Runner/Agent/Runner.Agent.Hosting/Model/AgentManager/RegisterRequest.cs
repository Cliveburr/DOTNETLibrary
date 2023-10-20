using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Hosting.Model.AgentManager
{
    public class RegisterRequest
    {
        public required string MachineName { get; set; }
        public required string AgentPool { get; set; }
        public required string AccessToken { get; set; }
        public required List<string> Tags { get; set; }
    }
}
