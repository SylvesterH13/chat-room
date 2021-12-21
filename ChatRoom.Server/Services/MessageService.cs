using ChatRoom.Server.Model;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatRoom.Server.Services
{
    public class MessageService : IMessageService
    {
        public Task SendMessageToAllAsync(IEnumerable<WebSocket> webSockets, ChatRoomPayloadData<SendMessageData> message)
        {
            return Task.WhenAll(
                webSockets
                    .Select(w => SendMessageAsync(w, message))
                );
        }

        public async Task SendMessageAsync(WebSocket webSocket, ChatRoomPayloadData<SendMessageData> message)
        {
            if (webSocket.State != WebSocketState.Open)
            {
                return;
            }

            var jsonString = JsonSerializer.Serialize(message);
            var encoded = Encoding.UTF8.GetBytes(jsonString);
            var arraySegment = new ArraySegment<byte>(encoded, offset: 0, count: encoded.Length);
            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
        }
    }
}
