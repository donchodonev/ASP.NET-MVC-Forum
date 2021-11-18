namespace ASP.NET_MVC_Forum.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Forum.Data.DataConstants.ChatConstants;


    public class Message
    {
        [Required]
        public long Id { get; set; }

        [MinLength(ChatMessageMinLength)]
        [MaxLength(ChatMessageMaxLength)]
        [Required]
        public string Text { get; set; }

        [Required]
        public long ChatId { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
