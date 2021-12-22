using ChatRoom.Server.Model;
using System.Net.WebSockets;

namespace ChatRoom.Server.Services
{
    public class CommandService : ICommandService
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public CommandService(IRoomService roomService, IUserService userService, IMessageService messageService)
        {
            _roomService = roomService;
            _userService = userService;
            _messageService = messageService;
        }

        public Task LoginAsync(WebSocket webSocket, LoginData loginData)
        {
            var user = _userService.Get(webSocket);
            user.Name = loginData.Name;
            return Task.CompletedTask;
        }

        public async Task EnterRoomAsync(WebSocket webSocket, RoomData roomData)
        {
            var user = _userService.Get(webSocket);
            var roomUsers = _roomService.GetUsers(roomData.RoomId);
            var enteringMessage = new ChatRoomPayloadData<SendMessageData>
            {
                Command = Command.SendMessage,
                Data = new SendMessageData
                {
                    Text = $"{user.Name} entrou na sala.",
                    RoomId = roomData.RoomId
                }
            };
            _roomService.EnterRoom(roomData.RoomId, user);
            await _messageService.SendMessageToAllAsync(roomUsers.Select(u => u.WebSocket), enteringMessage);
        }

        public async Task LeaveRoomAsync(WebSocket webSocket, RoomData roomData)
        {
            var user = _userService.Get(webSocket);
            var roomUsers = _roomService.GetUsers(roomData.RoomId);
            var leavingMessage = new ChatRoomPayloadData<SendMessageData>
            {
                Command = Command.SendMessage,
                Data = new SendMessageData
                {
                    Text = $"{user.Name} saiu da sala.",
                    RoomId = roomData.RoomId
                }
            };
            _roomService.LeaveRoom(roomData.RoomId, user);
            await _messageService.SendMessageToAllAsync(roomUsers.Select(u => u.WebSocket), leavingMessage);
        }

        public async Task SendMessageToRoomAsync(WebSocket webSocket, SendMessageData sendMessageData)
        {
            var user = _userService.Get(webSocket);
            var roomUsers = _roomService.GetUsers(sendMessageData.RoomId);
            var sendMessage = new ChatRoomPayloadData<SendMessageData>
            {
                Command = Command.SendMessage,
                Data = sendMessageData
            };
            await _messageService.SendMessageToAllAsync(roomUsers.Select(u => u.WebSocket), sendMessage);
        }
    }
}
