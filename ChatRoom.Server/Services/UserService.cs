using ChatRoom.Server.Model;
using System.Net.WebSockets;

namespace ChatRoom.Server.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>();
        }

        public void CreateAsync(WebSocket webSocket)
        {
            var user = new User(webSocket);
            _users.Add(user);
        }

        public void RemoveAsync(WebSocket webSocket)
        {
            var user = _users.Single(u => u.WebSocket == webSocket);
            _users.Remove(user);
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User Get(WebSocket webSocket)
        {
            return _users.Single(u => u.WebSocket == webSocket);
        }
    }
}
