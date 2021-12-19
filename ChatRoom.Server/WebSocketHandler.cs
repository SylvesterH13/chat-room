using ChatRoom.Server.Model;
using ChatRoom.Server.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatRoom.Server
{
    public class WebSocketHandler
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;

        public WebSocketHandler(IUserService userService, IRoomService roomService)
        {
            _userService = userService;
            _roomService = roomService;
        }
        public Task OnClosedAsync(WebSocket webSocket)
        {
            _userService.RemoveAsync(webSocket);
            return Task.CompletedTask;
        }

        public Task OnConnectedAsync(WebSocket webSocket)
        {
            _userService.CreateAsync(webSocket);
            return Task.CompletedTask;
        }

        public async Task OnReceivedAsync(WebSocket webSocket, WebSocketReceiveResult webSocketReceiveResult, byte[] buffer)
        {
            var jsonString = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
            if (jsonString is null)
            {
                throw new BadHttpRequestException("Payload message is empty.");
            }

            ChatRoomPayload chatRoomPayload;
            try
            {
                chatRoomPayload = JsonSerializer.Deserialize<ChatRoomPayload>(jsonString);
            }
            catch (Exception e) when (e is JsonException || e is NotSupportedException)
            {
                throw new BadHttpRequestException("Message is not in the correct format.");
            }

            if (chatRoomPayload.Command == Command.EnterRoom)
            {
                var enterRoomPayload = JsonSerializer.Deserialize<ChatRoomPayloadData<RoomData>>(jsonString);
                var roomId = enterRoomPayload.Data.RoomId;
                var user = _userService.Get(webSocket);
                var users = _roomService.GetUsers(roomId);
                var enteringMessage = new ChatRoomPayloadData<SendMessageData>
                {
                    Command = Command.SendMessage,
                    Data = new SendMessageData
                    {
                        Text = $"{user.Name} entrou na sala.",
                        RoomId = roomId
                    }
                };
                _roomService.EnterRoom(roomId, user);
                await SendMessageAsync(users.Select(u => u.WebSocket), enteringMessage);
            }

            if (chatRoomPayload.Command == Command.LeaveRoom)
            {
                var leaveRoomPayload = JsonSerializer.Deserialize<ChatRoomPayloadData<RoomData>>(jsonString);
                var roomId = leaveRoomPayload.Data.RoomId;
                var user = _userService.Get(webSocket);
                var users = _roomService.GetUsers(roomId);
                var leavingMessage = new ChatRoomPayloadData<SendMessageData>
                {
                    Command = Command.SendMessage,
                    Data = new SendMessageData
                    {
                        Text = $"{user.Name} saiu da sala.",
                        RoomId = roomId
                    }
                };
                _roomService.LeaveRoom(roomId, user);
                await SendMessageAsync(users.Select(u => u.WebSocket), leavingMessage);
            }

            if (chatRoomPayload.Command == Command.SendMessage)
            {
                var sendMessagePayload = JsonSerializer.Deserialize<ChatRoomPayloadData<SendMessageData>>(jsonString);
                var roomId = sendMessagePayload.Data.RoomId;
                var user = _userService.Get(webSocket);
                var users = _roomService.GetUsers(roomId);
                var sendMessage = new ChatRoomPayloadData<SendMessageData>
                {
                    Command = Command.SendMessage,
                    Data = new SendMessageData
                    {
                        Text = sendMessagePayload.Data.Text,
                        RoomId = roomId
                    }
                };
                await SendMessageAsync(users.Select(u => u.WebSocket), sendMessage);
            }
        }

        private Task SendMessageAsync(IEnumerable<WebSocket> webSockets, ChatRoomPayloadData<SendMessageData> message)
        {
            return Task.WhenAll(
                webSockets
                    .Where(w => w.State == WebSocketState.Open)
                    .Select(w => SendMessageAsync(w, message))
            );
        }

        private async Task SendMessageAsync(WebSocket webSocket, ChatRoomPayloadData<SendMessageData> message)
        {
            var jsonString = JsonSerializer.Serialize(message);
            var encoded = Encoding.UTF8.GetBytes(jsonString);
            var arraySegment = new ArraySegment<byte>(encoded, offset: 0, count: encoded.Length);
            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
        }
    }
}
