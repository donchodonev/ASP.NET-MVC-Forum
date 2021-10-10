namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Data.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataConstants.CommentConstants;
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

        [Required]
        public virtual User User { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        public bool IsVisible { get; set; }
    }
}
