using CSharpWeb.Kernel.Nodes;
using CSharpWeb.Kernel.Nodes.Builder;
using Microsoft.AspNetCore.Components;

namespace CSharpWeb.Kernel.Components;

public abstract class ComponentBase
{
    [Inject]
    public required NodeRefService NodeRefService { get; set; }

    public abstract Task Build(RenderBuilder builder);

    public RenderBuilder CreateRenderBuilder(Action<Node> buildAction)
    {
        var treeBuilder = new RenderBuilder(buildAction, NodeRefService);
        return treeBuilder;
    }
}
