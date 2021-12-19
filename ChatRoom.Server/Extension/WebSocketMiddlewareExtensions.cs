using ChatRoom.Server.Middleware;

namespace ChatRoom.Server.Extension
{
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ChatRoomWebSocketMiddleware>();
        }
    }
}
