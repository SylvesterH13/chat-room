using System.Net.WebSockets;

namespace ChatRoom.Server.Handlers
{
    public interface IWebSocketHandler
    {
        Task OnConnectedAsync(WebSocket webSocket);
        Task OnClosedAsync(WebSocket webSocket);
        Task OnReceivedAsync(WebSocket webSocket, WebSocketReceiveResult webSocketReceiveResult, byte[] buffer);
    }
}
