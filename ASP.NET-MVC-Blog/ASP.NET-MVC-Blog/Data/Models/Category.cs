namespace ASP.NET_MVC_Blog.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataConstants.CategoryConstants;

    public class Category : BaseModel
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

        public virtual ICollection<Post> Posts { get; set; }
    }
}
