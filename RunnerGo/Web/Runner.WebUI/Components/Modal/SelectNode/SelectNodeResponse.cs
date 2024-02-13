using Runner.Business.Entities.Nodes;

namespace Runner.WebUI.Components.Modal.SelectNode
{
    public class SelectNodeResponse
    {
        public required string NodePath { get; set; }
        public required Node Node { get; set; }
    }
}
