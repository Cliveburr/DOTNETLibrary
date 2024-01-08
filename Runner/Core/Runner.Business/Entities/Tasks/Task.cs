using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Tasks
{
    public class Task : DocumentBase
    {
        public TaskType Type { get; set; }
        public DateTime LastExecutionUTC { get; set; }
        public bool Enabled { get; set; }
        //public bool ForceRun { get; set; }
        //public bool StopRequested { get; set; }
    }
}
