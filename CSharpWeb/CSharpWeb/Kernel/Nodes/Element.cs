namespace CSharpWeb.Kernel.Nodes;

public class Element : Node
{
    public string TagName { get; set; }
    public Dictionary<string, string?>? Attributes { get; set; }
    public List<Node>? ChildNodes { get; set; }
    public string? TextContent { get; set; }
    public Node? ParentNode { get; set; }

    public Node(string tagName)
    {
        TagName = tagName;
    }

    public Node(string tagName, string? textContent)
    {
        TagName = tagName;
        TextContent = textContent;
    }

    public Node(string tagName, params Node[] childNodes)
    {
        TagName = tagName;
        ChildNodes = childNodes.ToList();
        foreach (var child in ChildNodes)
        {
            child.ParentNode = this;
        }
    }

    public virtual string Build()
    {
        var attrs = Attributes is null ?
            "" :
            $" {string.Join(' ', Attributes.Select(a =>
                a.Value is null ?
                $"{a.Key}" :
                $"{a.Key}=\"{a.Value}\""))}";

        if (TextContent is not null)
        {
            return $"<{TagName}{attrs}>{TextContent}</{TagName}>";
        }

        if (ChildNodes is not null)
        {
            var contents = ChildNodes
                .Select(x => x.Render());
            return $"<{TagName}{attrs}>{string.Concat(contents)}</{TagName}>";
        }

        return $"<{TagName}{attrs} />";
    }

    public void SetAttr(string name, string? value)
    {
        CheckAndSetAttr(name, value);
    }

    protected Node AddChildren(string name, string? content)
    {
        var node = new Node(name, content);
        CheckAndAddChildren(node);
        return node;
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

    private void CheckAndSetAttr(string name, string? value)
    {
        if (Attributes is null)
        {
            Attributes = new Dictionary<string, string?>
                {
                    { name, value }
                };
        }
        else
        {
            if (Attributes.ContainsKey(name))
            {
                Attributes[name] = value;
            }
            else
            {
                Attributes.Add(name, value);
            }
        }
    }
}
