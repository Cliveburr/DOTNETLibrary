namespace CSharpWeb.Kernel.Nodes.Builder;

public partial class RenderBuilder
{
    private NodeRefService _nodeRefService;
    private Action<Node>? _buildAction;

    public RenderBuilder(NodeRefService nodeRefService)
    {
        _nodeRefService = nodeRefService;
    }

    public async Task Build()
    {
        if (_buildAction is not null)
        {
            throw new InvalidOperationException("Build action is already set!");
        }
        await _buildAction.Invoke(new Node());
    }

    public override string Render()
    {
        var content = "";
        if (ChildNodes is not null)
        {
            foreach (var children in ChildNodes)
            {
                var thisContent = children.GetPlainHtml();
                content += thisContent;
            }
        }
        return content;
    }
}
