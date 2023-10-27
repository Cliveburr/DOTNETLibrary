using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public class Action
    {
        public int ActionId { get; set; }
        public required string Label { get; set; }
        public required string AgentPool { get; set; }
        public required List<string> Tags { get; set; }
        public ActionStatus Status { get; set; }
        public bool BreakPoint { get; set; }
    }
}
