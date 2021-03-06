namespace ASP.NET_MVC_Forum.Domain.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.PostConstants;

    public class Post : BaseModel
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
        [MaxLength(TITLE_MAX_LENGTH)]
        [MinLength(TITLE_MIN_LENGTH)]
        public string Title { get; set; }

        [Required]
        [MinLength(HTML_CONTENT_MIN_LENGTH)]
        public string HtmlContent { get; set; }

        public bool IsVisible { get; set; }

        public virtual int CategoryId { get; set; }

        public string ShortDescription { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public bool IsReported => Reports.Count > 0;

        public virtual ExtendedIdentityUser User { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<PostReport> Reports { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}