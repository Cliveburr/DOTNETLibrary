using Microsoft.JSInterop;
using System.Collections.Generic;


/*
         var styleClass = new StyleClass(".test", "test");
        styleClass.BackgroundColor = "red";

        var styleSheet = new StyleSheet("a");
        css.AddStyleSheet(styleSheet);
        styleSheet.AddClass(styleClass);

        await css.AddClass("a", ".test", styleClass.ToString());
 */

namespace Runner.WebUI.DynamicCSS
{
    public class CSSService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly List<StyleSheet> _styleSheets;

        public CSSService(IJSRuntime jsRuntime)
        {
            _styleSheets = new List<StyleSheet>();
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./dynamic-css.js").AsTask());
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        public async void AddStyleSheet(StyleSheet styleSheet)
        {
            if (await CreateStyleSheet(styleSheet.Id))
            {
                _styleSheets.Add(styleSheet);
            }
            else
            {
                throw new Exception("Invalid styleSheet! " + styleSheet.Id);
            }
        }

        public async ValueTask<bool> CreateStyleSheet(string id)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("createStyleSheet", id);
        }

        public async ValueTask<bool> AddClass(string id, string selector, string rules)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("addClass", id, selector, rules);
        }
    }
}
