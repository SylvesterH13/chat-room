using System.Net.WebSockets;

namespace ChatRoom.Server.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ChatRoomWebSocketMiddleware
    {
        private const string WEB_SOCKET_PATH = "/ws";
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _webSocketHandler;

        public ChatRoomWebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext httpContext)
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
            await _webSocketHandler.OnConnectedAsync(webSocket);

            await Receive(webSocket);
        }

        private async Task Receive(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
            {
                await _webSocketHandler.OnReceivedAsync(webSocket, webSocketReceiveResult, buffer);

                webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the client.", CancellationToken.None);
            await _webSocketHandler.OnClosedAsync(webSocket);
        }
    }
}
