namespace ASP.NET_MVC_Forum.Models.Chat
{
    public class ChatConversationViewModel
    {
        public ChatConversationViewModel()
        {

        }

        public ChatConversationViewModel(long chatId, string senderIdentityUserId, string receiverIdentityUserId)
        {
            ChatId = chatId;
            SenderIdentityUserId = senderIdentityUserId;
            ReceiverIdentityUserId = receiverIdentityUserId;
        }

        public long ChatId { get; set; }
        public string SenderIdentityUserId { get; set; }
        public string ReceiverIdentityUserId { get; set; }
    }
}
