using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Model
{
    public class GetScriptResponse
    {
        public required List<GetScriptFile> Files { get; set; }
    }

    public class GetScriptFile
    {
        public required string Name { get; set; }
        public required byte[] Content { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
