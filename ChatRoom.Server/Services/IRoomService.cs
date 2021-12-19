using ChatRoom.Server.Model;

namespace ChatRoom.Server.Services
{
    public interface IRoomService
    {
        void EnterRoom(int roomId, User user);
        void LeaveRoom(int roomId, User user);
        IEnumerable<User> GetUsers(int id);
    }
}
