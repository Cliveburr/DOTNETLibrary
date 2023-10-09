using Microsoft.JSInterop;
using Runner.WebUI.DynamicCSS;

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
    }


    //public class GlobalJavascript : IAsyncDisposable
    //{
    //    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    //    public GlobalJavascript(IJSRuntime jsRuntime)
    //    {
    //        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
    //            "import", "./global.js").AsTask());
    //    }

    //    public async ValueTask DisposeAsync()
    //    {
    //        if (moduleTask.IsValueCreated)
    //        {
    //            var module = await moduleTask.Value;
    //            await module.DisposeAsync();
    //        }
    //    }

    //    public async ValueTask<string?> GetStorage(string key)
    //    {
    //        var module = await moduleTask.Value;
    //        return await module.InvokeAsync<string?>("getStorage", key);
    //    }

    //    public async ValueTask SetStorage(string key, string value)
    //    {
    //        var module = await moduleTask.Value;
    //        await module.InvokeVoidAsync("setStorage", key, value);
    //    }

    //    public async ValueTask RemoveStorage(string key)
    //    {
    //        var module = await moduleTask.Value;
    //        await module.InvokeVoidAsync("removeStorage", key);
    //    }
    //}
}
