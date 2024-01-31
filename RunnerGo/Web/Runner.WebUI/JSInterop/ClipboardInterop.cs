using Microsoft.JSInterop;

namespace Runner.WebUI.JSInterop
{
    public class ClipboardInterop
    {
        private readonly IJSRuntime _jsInterop;

        public ClipboardInterop(IJSRuntime jsInterop)
        {
            _jsInterop = jsInterop;
        }

        public async Task WriteTextAsync(string text)
        {
            await _jsInterop.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }

        public ValueTask<string> ReadTextAsync()
        {
            return _jsInterop.InvokeAsync<string>("navigator.clipboard.readText");
        }
    }
}
