using CSharpWeb.Kernel.Nodes.Builder;

namespace CSharpWeb.Kernel.Nodes;

public class Node
{
    public List<Node>? ChildNodes { get; set; }
    public Node? ParentNode { get; set; }

    public virtual async Task Build(RenderBuilder builder)
    {
        if (ChildNodes is not null)
        {
            foreach (var child in ChildNodes)
            {
                await child.Build(builder);
            }
        }
    }

    public virtual string Render()
    {
        return string.Empty;
    }

    protected Node AddChildren(Node node)
    {
        CheckAndAddChildren(node);
        return node;
    }

    private void CheckAndAddChildren(Node node)
    {
        if (ChildNodes is not null)
        {
            throw new InvalidOperationException("Cannot add children with static content!");
        }
        if (ChildNodes is null)
        {
            ChildNodes = new List<Node>
                {
                    node
                };
        }
        else
        {
            ChildNodes.Add(node);
        }
    }
}
