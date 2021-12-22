using ChatRoom.Server.Model;
using ChatRoom.Server.Services;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatRoom.Server.Handlers
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IUserService _userService;
        private readonly ICommandService _commandService;

        public WebSocketHandler(IUserService userService, ICommandService commandService)
        {
            _userService = userService;
            _commandService = commandService;
        }

        public Task OnConnectedAsync(WebSocket webSocket)
        {
            _userService.CreateAsync(webSocket);
            return Task.CompletedTask;
        }

        public Task OnClosedAsync(WebSocket webSocket)
        {
            _userService.RemoveAsync(webSocket);
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

            if (chatRoomPayload.Command == Command.Login)
            {
                var loginPayload = JsonSerializer.Deserialize<ChatRoomPayloadData<LoginData>>(jsonString);
                await _commandService.LoginAsync(webSocket, loginPayload.Data);
            }

            if (chatRoomPayload.Command == Command.EnterRoom)
            {
                var enterRoomPayload = JsonSerializer.Deserialize<ChatRoomPayloadData<RoomData>>(jsonString);
                await _commandService.EnterRoomAsync(webSocket, enterRoomPayload.Data);
            }

            if (chatRoomPayload.Command == Command.SendMessage)
            {
                var sendMessagePayload = JsonSerializer.Deserialize<ChatRoomPayloadData<SendMessageData>>(jsonString);
                await _commandService.SendMessageToRoomAsync(webSocket, sendMessagePayload.Data);
            }

            if (chatRoomPayload.Command == Command.LeaveRoom)
            {
                var leaveRoomPayload = JsonSerializer.Deserialize<ChatRoomPayloadData<RoomData>>(jsonString);
                await _commandService.LeaveRoomAsync(webSocket, leaveRoomPayload.Data);
            }
        }
    }
}
