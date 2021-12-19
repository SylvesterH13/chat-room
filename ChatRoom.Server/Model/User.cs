using System.Net.WebSockets;

namespace ChatRoom.Server.Model
{
    public class User
    {
        public string Name { get; set; }
        public WebSocket WebSocket { get; }

        public User(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }
    }
}
