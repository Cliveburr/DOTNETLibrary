using Runner.Script.Interface.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Version.Isolation
{
    public record ExecuteResult
    {
        public bool IsSuccess { get; set; }
        public bool ContinueOnError { get; set; }
        public string? ErrorMessage { get; set; }
        public DataWriter? Output { get; set; }
    }
}
