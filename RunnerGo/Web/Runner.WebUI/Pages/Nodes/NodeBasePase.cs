using Microsoft.AspNetCore.Components;
using Runner.Business.Entities.Nodes;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Runner.WebUI.Pages.Nodes
{
    public class NodeBasePage : BasePage
    {
        [Parameter]
        public required Node Node { get; set; }

        public void FowardNode(Node node)
        {
            FowardNode(node.Name);
        }

        public Task FavoriteThisNode()
        {
            return UserHomeService.CheckAndAddNodeFavorite(Node.Name, Node.Type.ToString(), Node.Type, Node.NodeId);
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
