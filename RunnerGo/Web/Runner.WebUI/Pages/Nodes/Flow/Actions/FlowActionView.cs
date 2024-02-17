using Microsoft.AspNetCore.Components;
using Runner.Business.Entities.Nodes.Types;
using Runner.WebUI.Components.Panels;
using Runner.WebUI.Pages;

namespace Runner.WebUI.Pages.Nodes.Flow.Actions
{
    public abstract class FlowActionView : BasePage
    {
        [Parameter]
        public required FlowAction Node { get; set; }

        [Parameter]
        public FlowActionView? Parent { get; set; }

        [Parameter]
        public required Action UpdateParent { get; set; }

        [CascadingParameter(Name = "View")]
        public required FlowView View { get; set; }

        protected abstract RenderFragment EditorForm();

        protected virtual string ValidLabel()
        {
            return string.IsNullOrEmpty(Node.Label) ?
                " " :
                Node.Label;
        }

        protected void OnOpenEditor()
        {
            if (View.Panel is not null)
            {
                View.Panel.SetOpen(EditorForm());
            }
        }

        protected void ClosePanel()
        {
            if (View.Panel is not null)
            {
                View.Panel.CleanPanel();
            }
        }

        protected void AdjustValueKind(FlowAction? action)
        {
            //if (action is null)
            //{
            //    return;
            //}
            //if (action.Data is not null)
            //{
            //    foreach (var data in action.Data)
            //    {
            //        if (data.Value is not null)
            //        {
            //            var value = (System.Text.Json.JsonElement)data.Value;
            //            data.Value = data.Type switch
            //            {
            //                Business.Datas.Model.DataTypeEnum.String => value.GetString(),
            //                Business.Datas.Model.DataTypeEnum.StringList => value.EnumerateArray()
            //                    .Select(e => e.GetString())
            //                    .ToList(),
            //                Business.Datas.Model.DataTypeEnum.NodePath => value.GetString(),
            //                Business.Datas.Model.DataTypeEnum.Reference => value.GetString(),
            //                _ => data.Value
            //            };
            //        }
            //    }
            //}
            //if (action.Childs is not null)
            //{
            //    foreach (var child in action.Childs)
            //    {
            //        AdjustValueKind(child);
            //    }
            //}
        }

        protected void UpdateState()
        {
            StateHasChanged();
        }

        public virtual void RemoveChild(FlowAction node)
        {
        }
        
        public virtual void MoveUp(FlowAction node)
        {
        }

        public virtual void MoveDown(FlowAction node)
        {
        }
    }
}
