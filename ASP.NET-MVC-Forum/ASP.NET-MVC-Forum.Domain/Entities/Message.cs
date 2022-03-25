namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Domain.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.ChatConstants;

    public class Message : ICreatedOn
    {
        public Message()
        {
            CreatedOn = DateTime.UtcNow;
        }

        [Required]
        public long Id { get; set; }

        [MinLength(ChatMessageMinLength)]
        [MaxLength(ChatMessageMaxLength)]
        [Required]
        public string Text { get; set; }

        [Required]
        public long ChatId { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        public DateTime CreatedOn { get; init; }

        public virtual Chat Chat { get; set; }
    }
}