using Runner.Business.Entities.Nodes;
using Runner.Business.Entities.Nodes.Types;

namespace Runner.Business.Model.Nodes.Types
{
    public class AppView
    {
        public required App App { get; set; }
        public required Node Node { get; set; }
    }
}
