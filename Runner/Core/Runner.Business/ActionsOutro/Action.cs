using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro
{
    public class Action
    {
        public int Id { get; set; }
        public required string Label { get; set; }
        public required string AgentPool { get; set; }
        public required List<string> Tags { get; set; }
        public ActionStatus Status { get; set; }
        public bool BreakPoint { get; set; }
        public List<int>? Content { get; set; }
        public List<int>? Next { get; set; }
    }
}
