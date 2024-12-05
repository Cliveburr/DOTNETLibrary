namespace CSharpWeb.Kernel.Nodes.Builder;

public partial class RenderBuilder
{
    public RenderBuilder Node(string tag, Action<RenderBuilder> action)
    {
        AddChildren(new NodeCommand(this, tag, action));
        return this;
    }

}
