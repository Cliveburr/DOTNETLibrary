using Runner.Business.Entities.Node;

namespace Runner.WebUI.Components.FlowEditor
{
    public interface IFlowActionView
    {
        void RemoveChild(FlowAction node);
        void UpdateState();
        void MoveUp(FlowAction node);
        void MoveDown(FlowAction node);
    }
}
