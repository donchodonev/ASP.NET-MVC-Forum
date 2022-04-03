namespace ASP.NET_MVC_Forum.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.CommentConstants;
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

        public string UserId { get; set; }

        public virtual ExtendedIdentityUser User { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        public bool IsVisible { get; set; }
    }
}
