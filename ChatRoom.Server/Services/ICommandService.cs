using ChatRoom.Server.Model;
using System.Net.WebSockets;

namespace ChatRoom.Server.Services
{
    public interface ICommandService
    {
        Task LoginAsync(WebSocket webSocket, LoginData loginData);
        Task EnterRoomAsync(WebSocket webSocket, RoomData roomData);
        Task LeaveRoomAsync(WebSocket webSocket, RoomData roomData);
        Task SendMessageToRoomAsync(WebSocket webSocket, SendMessageData sendMessageData);
    }
}
