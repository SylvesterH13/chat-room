using ChatRoom.Server.Model;
using System.Net.WebSockets;

namespace ChatRoom.Server.Services
{
    public interface IUserService
    {
        void CreateAsync(WebSocket webSocket);
        void RemoveAsync(WebSocket webSocket);
        IEnumerable<User> GetAll();
        User Get(WebSocket webSocket);
    }
}
