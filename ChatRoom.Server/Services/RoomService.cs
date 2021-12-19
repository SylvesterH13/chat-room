using ChatRoom.Server.Model;

namespace ChatRoom.Server.Services
{
    public class RoomService : IRoomService
    {
        private readonly List<Room> _rooms;

        public RoomService()
        {
            _rooms = new List<Room>()
            {
                new Room { Id = 1, Name = "Teste", Users = new List<User>() }
            };
        }

        public void EnterRoom(int roomId, User user)
        {
            var room = _rooms.Single(r => r.Id == roomId);
            room.Users.Add(user);
        }

        public void LeaveRoom(int roomId, User user)
        {
            var room = _rooms.Single(r => r.Id == roomId);
            room.Users.Remove(user);
        }

        public IEnumerable<User> GetUsers(int id)
        {
            return _rooms.Single(r => r.Id == id).Users;
        }
    }
}
