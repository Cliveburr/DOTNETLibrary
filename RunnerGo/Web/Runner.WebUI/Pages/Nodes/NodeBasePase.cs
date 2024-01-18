using Microsoft.AspNetCore.Components;
using Runner.Business.Entities.Nodes;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Runner.WebUI.Pages.Nodes
{
    public class NodeBasePase : BasePage
    {
        [Parameter]
        public required Node Node { get; set; }

        public void FowardNode(Node node)
        {
            FowardNode(node.Name);
        }

        public void FowardNode(string name)
        {
            var parts = new List<string>(NavigationManager.Uri.Substring(NavigationManager.BaseUri.Length)
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            parts.Add(name);
            NavigationManager.NavigateTo($"/{string.Join('/', parts)}", false, true);
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
