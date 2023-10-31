using Microsoft.AspNetCore.Components;
using Runner.WebUI.Components.Modal;
using Runner.WebUI.Components.Notification;
using System.IO;

namespace Runner.WebUI.Components
{
    public class BaseService
    {
        public ModalService Modal { get; private set; }
        public NotificationService Notification { get; private set; }
        public NavigationManager Navigation { get; private set; }

        public BaseService(ModalService modal, NotificationService notification, NavigationManager navigationManager)
        {
            Modal = modal;
            Notification = notification;
            Navigation = navigationManager;
        }

        public void FowardNode(string path)
        {
            var parts = new List<string>(Navigation.Uri.Substring(Navigation.BaseUri.Length)
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            parts.Add(path);
            Navigation.NavigateTo($"/{string.Join('/', parts)}", false, true);
        }

        public void BackNode()
        {
            var parts = new List<string>(Navigation.Uri.Substring(Navigation.BaseUri.Length)
                .Split("/", StringSplitOptions.RemoveEmptyEntries));
            parts.RemoveAt(parts.Count - 1);
            Navigation.NavigateTo($"/{string.Join('/', parts)}", false, true);
        }

        public void Reload()
        {
            Navigation.NavigateTo(Navigation.Uri, false, true);
        }
    }
}
