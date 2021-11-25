namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Data.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Constants.DataConstants.CategoryConstants;

    public class Category : BaseModel, IContainImage
    {
        public Category()
            :base()
        {
            Posts = new HashSet<Post>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
