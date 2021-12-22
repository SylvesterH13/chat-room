using ChatRoom.Server.Handlers;
using System.Net.WebSockets;

namespace ChatRoom.Server.Middleware
{
    public class ChatRoomWebSocketMiddleware
    {
        private const string WEB_SOCKET_PATH = "/ws";
        private readonly RequestDelegate _next;

        public ChatRoomWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IWebSocketHandler webSocketHandler)
        {
            if (httpContext.Request.Path != WEB_SOCKET_PATH)
            {
                await _next(httpContext);
                return;
            }

            if (!httpContext.WebSockets.IsWebSocketRequest)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
            await webSocketHandler.OnConnectedAsync(webSocket);

            await Receive(webSocket, webSocketHandler);
        }

        private static async Task Receive(WebSocket webSocket, IWebSocketHandler webSocketHandler)
        {
            var buffer = new byte[1024 * 4];
            var webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
            {
                await webSocketHandler.OnReceivedAsync(webSocket, webSocketReceiveResult, buffer);

                webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client.", CancellationToken.None);
            await webSocketHandler.OnClosedAsync(webSocket);
        }
    }
}
