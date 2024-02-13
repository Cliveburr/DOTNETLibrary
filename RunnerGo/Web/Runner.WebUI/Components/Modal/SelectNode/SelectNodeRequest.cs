using Runner.Business.Entities.Nodes;

namespace Runner.WebUI.Components.Modal.SelectNode
{
    public class SelectNodeRequest
    {
        public required string Title { get; set; }
        public required string Value { get; set; }
        public string? PlaceHolder { get; set; }
        public List<NodeType>? AllowedTypes { get; set; }
    }
}
