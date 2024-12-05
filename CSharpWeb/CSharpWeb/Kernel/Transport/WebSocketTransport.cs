using CSharpWeb.Kernel.Nodes;
using System.Net.WebSockets;
using System.Text;

namespace CSharpWeb.Kernel.Transport;

public class WebSocketTransport
{
    private readonly WebSocket _ws;
    private readonly NodeRefService _nodeRefService;

    public WebSocketTransport(WebSocket ws, NodeRefService nodeRefService)
    {
        _ws = ws;
        _nodeRefService = nodeRefService;
    }

    internal Task BeginReceiveAsync()
    {
        return BeginReceive();
    }

    private async Task BeginReceive()
    {
        var buffer = new byte[1024 * 4];
        while (_ws.State == WebSocketState.Open)
        {
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            await HandleMessage(result, buffer);
        }
    }

    private async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer)
    {
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var id = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //_hookService.Invoke(id);
        }
        else if (result.MessageType == WebSocketMessageType.Close || _ws.State == WebSocketState.Aborted)
        {
            await _ws.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);

            // release the session
        }
    }

    public Task SendAsync(string msg)
    {
        var buffer = Encoding.UTF8.GetBytes(msg);

        return _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
