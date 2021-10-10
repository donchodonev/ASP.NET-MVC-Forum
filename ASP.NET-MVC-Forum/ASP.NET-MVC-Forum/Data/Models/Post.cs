namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Data.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.PostConstants;

    public class Post : BaseModel, IContainImage
    {
        public Post()
            :base()
        {
            Comments = new HashSet<Comment>();
            IsVisible = true;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [MinLength(NameMinLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(HtmlContentMaxLength)]
        [MinLength(HtmlContentMinLength)]
        public string HtmlContent { get; set; }

        public bool IsVisible { get; set; }

        [Required]
        public virtual int CategoryId { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}