using Microsoft.AspNetCore.Components;
using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using Runner.WebUI.JSInterop;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Runner.WebUI.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected GlobalJavascript JS { get; set; }

        [Inject]
        protected ModalService Modal { get; set; }

        [Inject]
        protected NotificationService Notification { get; set; }

        protected void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            NavigationManager.NavigateTo(uri, forceLoad, replace);
        }

        public void Reload()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, true, true);
        }

        public Task HistoryBack()
        {
            return JS.HistoryBack().AsTask();
        }

        public void NodeBack()
        {
            var parts = new List<string>(NavigationManager.Uri.Substring(NavigationManager.BaseUri.Length)
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            parts.RemoveAt(parts.Count - 1);
            NavigationManager.NavigateTo($"/{string.Join('/', parts)}", false, true);
        }

        public void PageNotFound()
        {
            NavigationManager.NavigateTo("/"); //TODO: improve this
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
