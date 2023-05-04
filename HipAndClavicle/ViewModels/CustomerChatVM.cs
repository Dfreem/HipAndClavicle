using System;
namespace HipAndClavicle.ViewModels
{

    public class ChatVM
    {
        public List<ChatMessage> Messages { get; set; }
        public List<ChatUser> Users { get; set; }
        public string CurrentUserName { get; set; }
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; }
        public string ToUserName { get; set; }
    }


    public class ChatUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }

}