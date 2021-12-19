using System.Text.Json.Serialization;

namespace ChatRoom.Server.Model
{
    public class SendMessageData
    {
        public string Text { get; set; }
        public int RoomId { get; set; }
    }
}
