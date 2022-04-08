namespace ASP.NET_MVC_Forum.Domain.Entities
{
    using ASP.NET_MVC_Forum.Domain.Interfaces;

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.CategoryConstants;

    public class Category : BaseModel, IContainImage
    {
        public Category()
            : base()
        {
            Posts = new HashSet<Post>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NAME_MAX_LENGTH)]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
