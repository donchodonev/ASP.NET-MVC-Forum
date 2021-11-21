namespace ASP.NET_MVC_Forum.Models.Chat
{
    public class ChatConversationViewModel
    {
        public ChatConversationViewModel()
        {

        }

        public ChatConversationViewModel(long chatId, string senderIdentityUserId, string receiverIdentityUserId, string senderUsername)
        {
            ChatId = chatId;
            SenderIdentityUserId = senderIdentityUserId;
            ReceiverIdentityUserId = receiverIdentityUserId;
            SenderUsername = senderUsername;
        }

        public long ChatId { get; set; }

        public string SenderIdentityUserId { get; set; }

        public string ReceiverIdentityUserId { get; set; }

        public string SenderUsername { get; set; }
    }
}
