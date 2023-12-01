using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Model
{
    public class GetScriptRequest
    {
        public required string Id { get; set; }
        public required int Version { get; set; }
    }
}
