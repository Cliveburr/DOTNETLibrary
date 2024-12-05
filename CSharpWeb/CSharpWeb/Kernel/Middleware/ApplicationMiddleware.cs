using CSharpWeb.Kernel.Components;
using CSharpWeb.Kernel.Nodes;
using CSharpWeb.Kernel.Session;

namespace CSharpWeb.Kernel.Middleware;

public class ApplicationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SessionService _sessionService;

    public ApplicationMiddleware(RequestDelegate next, SessionService sessionService)
    {
        _next = next;
        _sessionService = sessionService;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            if (await WebSocket(ctx))
            {
            }
            else
            {
                await FirstCall(ctx);
            }
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = 500;
            await ctx.Response.WriteAsync(ex.ToString());
        }

        await _next(ctx);
    }

    private async Task FirstCall(HttpContext ctx)
    {
        var session = _sessionService.Create();
        var sessionProvider = session.Scope.ServiceProvider.GetRequiredService<SessionProvider>();
        sessionProvider.Session = session;

        var app = session.Scope.ServiceProvider.GetRequiredKeyedService<ComponentBase>("app");

        var builder = app.CreateRenderBuilder(n =>
            n.AddElement("html"));
            //.Node("html", html =>
            //    html
            //        .Node("header", "")
            //        .Node("body", app.Render)
            //    //.PlainScript($"window.sessionId = {session.SessionId};")
            //    //.Script("/websocket.js")
            //);

        await builder.Build();

        var plainHtml = builder.Render();

        await ctx.Response.WriteAsync(plainHtml);
    }

    private async Task<bool> WebSocket(HttpContext ctx)
    {
        if (ctx.Request.Path != "/_ws" || !ctx.WebSockets.IsWebSocketRequest)
        {
            return false;
        }

        var sessionIdStc = ctx.Request.Query["sessionId"];
        if (!int.TryParse(sessionIdStc, out var sessionId))
        {
            throw new Exception($"Session not recognized!");
        }

        var session = _sessionService.Get(sessionId);

        var ws = await ctx.WebSockets.AcceptWebSocketAsync();
        var nodeRefService = session.Scope.ServiceProvider.GetRequiredService<NodeRefService>();
        var transport = nodeRefService.SetWebSocketTransport(ws);

        await transport.BeginReceiveAsync();

        return true;
    }
}
