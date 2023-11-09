using MongoDB.Bson.Serialization.Attributes;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public class ActionContainer
    {
        public int ActionContainerId { get; set; }
        public required string Label { get; set; }
        public ActionContainerStatus Status { get; set; }
        public int Position { get; set; }
        public bool IsForActions { get; set; }
        public List<int>? Actions { get; set; }
        public List<int>? ContentContainers { get; set; }
        public List<int>? Next { get; set; }
    }
}
