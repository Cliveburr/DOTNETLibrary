using Microsoft.AspNetCore.Components;

namespace Runner.WebUI.Components.Modal
{
    public class ModalBase : ComponentBase
    {
        [Parameter]
        public required ModalControl Control { get; set; }

        protected void Resume(object response)
        {
            Control.Response = response;
            Control.Resume.Set();
        }

        protected void Close()
        {
            Control.Response = null;
            Control.Resume.Set();
        }
    }
}
