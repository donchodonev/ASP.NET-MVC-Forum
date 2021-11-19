namespace ASP.NET_MVC_Forum.Models.Chat
{
    public class ChatSelectUserViewModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Username { get; set; }

        public string RecipientIdentityUserId { get; set; }

        public string SenderIdentityUserId { get; set; }
    }
}
