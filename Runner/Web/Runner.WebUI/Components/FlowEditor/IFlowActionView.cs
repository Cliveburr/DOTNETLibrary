using Runner.Business.Entities;

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
