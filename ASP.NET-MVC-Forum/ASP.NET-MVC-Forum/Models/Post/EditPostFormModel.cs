namespace ASP.NET_MVC_Forum.Models.Post
{
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants.PostConstants;

    public class EditPostFormModel
    {
        public CategoryIdAndNameViewModel[] Categories { get; set; }

        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [MinLength(HtmlContentMinLength)]
        public string HtmlContent { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CategoryId { get; set; }

        public int PostId { get; set; }

        public void FillCategories(ICategoryDataService categoryService) 
            => Categories = categoryService.GetCategoryIdAndNameCombinations();
    }
}
