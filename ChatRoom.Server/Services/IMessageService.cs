using ChatRoom.Server.Model;
using System.Net.WebSockets;

namespace ChatRoom.Server.Services
{
    public interface IMessageService
    {
        Task SendMessageToAllAsync(IEnumerable<WebSocket> webSockets, ChatRoomPayloadData<SendMessageData> message);
        Task SendMessageAsync(WebSocket webSocket, ChatRoomPayloadData<SendMessageData> message);
    }
}
