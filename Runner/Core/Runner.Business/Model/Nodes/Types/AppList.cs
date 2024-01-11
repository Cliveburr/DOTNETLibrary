using Runner.Business.Entities.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Model.Nodes.Types
{
    public class AppList
    {
        public required string Name { get; set; }
        public NodeType Type { get; set; }
    }
}
