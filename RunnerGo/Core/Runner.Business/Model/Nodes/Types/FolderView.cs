using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes;

namespace Runner.Business.Model.Nodes.Types
{
    public class FolderView
    {
        public required Folder Folder { get; set; }
        public required Node Node { get; set; }
    }
}
