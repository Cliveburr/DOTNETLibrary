using Microsoft.JSInterop;

namespace Runner.WebUI.Helpers
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
    }
}
