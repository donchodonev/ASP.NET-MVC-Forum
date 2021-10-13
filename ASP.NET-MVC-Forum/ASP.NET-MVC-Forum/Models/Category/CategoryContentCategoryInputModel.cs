namespace ASP.NET_MVC_Forum.Models.Category
{
    using System.ComponentModel.DataAnnotations;

    public class CategoryContentCategoryInputModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int CategoryName { get; set; }
    }
}
