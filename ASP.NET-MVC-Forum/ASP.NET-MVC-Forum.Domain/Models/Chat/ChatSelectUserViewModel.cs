namespace ASP.NET_MVC_Forum.Domain.Models.Chat
{
    public class ChatSelectUserViewModel
    {
        public string Id { get; set; }

        public string ImageUrl { get; set; }

        public string SenderUsername { get; set; }

        public string SenderIdentityUserId { get; set; }

        public string RecipientUsername { get; set; }

        public string RecipientIdentityUserId { get; set; }
    }
}
