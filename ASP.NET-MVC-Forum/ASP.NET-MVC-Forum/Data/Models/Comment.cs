namespace ASP.NET_MVC_Forum.Web.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static Constants.DataConstants.CommentConstants;
    public class Comment : BaseModel
    {
        public Comment() 
            : base()
        {
            IsVisible = true;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(ContentMaxLength)]
        [MinLength(ContentMinLength)]
        public string Content { get; set; }

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        public bool IsVisible { get; set; }
    }
}
