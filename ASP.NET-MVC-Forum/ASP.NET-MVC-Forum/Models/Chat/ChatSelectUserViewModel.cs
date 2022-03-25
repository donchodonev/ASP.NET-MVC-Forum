﻿namespace ASP.NET_MVC_Forum.Web.Models.Chat
{
    public class ChatSelectUserViewModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string SenderUsername { get; set; }

        public string SenderIdentityUserId { get; set; }

        public string RecipientUsername { get; set; }

        public string RecipientIdentityUserId { get; set; }

    }
}
