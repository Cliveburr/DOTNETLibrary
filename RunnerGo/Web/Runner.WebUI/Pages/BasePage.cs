using Microsoft.AspNetCore.Components;
using Runner.WebUI.Helpers;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Runner.WebUI.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected GlobalJavascript JS { get; set; }

        protected void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            NavigationManager.NavigateTo(uri, forceLoad, replace);
        }

        public void Reload()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, false, true);
        }

        public Task HistoryBack()
        {
            return JS.HistoryBack().AsTask();
        }
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
