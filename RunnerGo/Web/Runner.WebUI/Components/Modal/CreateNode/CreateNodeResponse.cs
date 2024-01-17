using Runner.Business.Entities.Nodes;

namespace Runner.WebUI.Components.Modal.CreateNode
{
    public class CreateNodeResponse
    {
        public string? Name { get; set; }
        public NodeType NodeType { get; set; }
    }
}
