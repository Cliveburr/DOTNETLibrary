using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Runner.Business.Actions
{
    public class ActionContainer
    {
        public int ActionContainerId { get; set; }
        public required string Label { get; set; }
        public ActionContainerStatus Status { get; set; }
        public int Position { get; set; }
        public required List<int> Actions { get; set; }
        public required List<int> Next { get; set; }
    }
}
