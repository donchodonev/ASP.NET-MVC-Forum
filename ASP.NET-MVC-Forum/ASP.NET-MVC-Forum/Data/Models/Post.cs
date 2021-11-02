namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Data.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataConstants.PostConstants;

    public class Post : BaseModel, IContainImage
    {
        public Post()
            :base()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
            Reports = new HashSet<PostReport>();
            IsVisible = true;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        [MinLength(TitleMinLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(HtmlContentMinLength)]
        public string HtmlContent { get; set; }

        public bool IsVisible { get; set; }

        [Required]
        public virtual int CategoryId { get; set; }

        public string ShortDescription { get; set; }

        public string ImageUrl { get; set; }
        
        public int? UserId { get; set; }

        [NotMapped]
        public bool IsReported => Reports.Count > 0;

        public virtual User User { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<PostReport> Reports { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}