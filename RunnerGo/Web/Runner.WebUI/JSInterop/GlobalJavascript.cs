using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.JSInterop;
using Runner.WebUI.Components.Inputs;

namespace Runner.WebUI.JSInterop
{
    public class GlobalJavascript
    {
        private IJSRuntime _js;

        public GlobalJavascript(IJSRuntime jsRuntime)
        {
            _js = jsRuntime;
        }

        public ValueTask<string?> GetStorage(string key)
        {
            return _js.InvokeAsync<string?>("globalJS.getStorage", key);
        }

        public Task SetStorage(string key, string value)
        {
            return _js.InvokeVoidAsync("globalJS.setStorage", key, value).AsTask();
        }

        public ValueTask RemoveStorage(string key)
        {
            return _js.InvokeVoidAsync("globalJS.removeStorage", key);
        }

        public ValueTask HistoryBack()
        {
            return _js.InvokeVoidAsync("history.back");
        }

        // type = date|time|datetime
        // displayMode = default|dialog|inline
        public Task AttachCalendar(ElementReference el, DotNetObjectReference<DateTimeInput>? elRef, string requestStr, string type, string displayMode)
        {
            return _js.InvokeVoidAsync("globalJS.attachCalendar", el, elRef, requestStr, type, displayMode).AsTask();
        }

        public Task UpdateCalendar(ElementReference el, string requestStr)
        {
            return _js.InvokeVoidAsync("globalJS.attachCalendar", el, requestStr).AsTask();
        }
    }
}
