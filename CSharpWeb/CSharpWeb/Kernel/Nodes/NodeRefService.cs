using CSharpWeb.Kernel.Transport;
using System.Net.WebSockets;

namespace CSharpWeb.Kernel.Nodes;

public class NodeRefService
{
    private WebSocketTransport? _transport;
    private int _nextEventId;

    internal WebSocketTransport SetWebSocketTransport(WebSocket ws)
    {
        _transport = new WebSocketTransport(ws, this);
        return _transport;
    }
}
