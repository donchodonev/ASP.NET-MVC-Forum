using ASP.NET_MVC_Forum.Models.Category;
using System.ComponentModel.DataAnnotations;

using static ASP.NET_MVC_Forum.Data.DataConstants.PostConstants;
namespace ASP.NET_MVC_Forum.Models.Post
{
    public class AddPostFormModel
    {
        public  CategoryIdAndName[] Categories { get; set; }

        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [MinLength(HtmlContentMinLength)]
        [DataType(DataType.MultilineText)]
        public string HtmlContent { get; set; }

        [Required]
        [Range(0,int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
