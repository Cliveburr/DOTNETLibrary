using Runner.Business.Entities.Nodes.Types;

namespace Runner.WebUI.Components.FlowEditor
{
    public interface IFlowActionView
    {
        void RemoveChild(FlowAction node);
        void MoveUp(FlowAction node);
        void MoveDown(FlowAction node);
    }
}
