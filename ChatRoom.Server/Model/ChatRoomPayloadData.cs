namespace ChatRoom.Server.Model
{
    public class ChatRoomPayloadData<T> : ChatRoomPayload
    {
        public T Data { get; set; }
    }
}
