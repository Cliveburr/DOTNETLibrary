using Runner.Script.Interface.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Interface.Model
{
    public class ScriptFinishRequest
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public List<DataWriterChanges>? Output { get; set; }
    }
}
