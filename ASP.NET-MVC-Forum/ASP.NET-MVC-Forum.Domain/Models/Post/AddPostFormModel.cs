namespace ASP.NET_MVC_Forum.Domain.Models.Post
{
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.PostConstants;

    public class AddPostFormModel
    {
        public CategoryIdAndNameViewModel[] Categories { get; set; }

        [Required]
        [MinLength(TITLE_MIN_LENGTH)]
        [MaxLength(TITLE_MAX_LENGTH)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [MinLength(HTML_CONTENT_MIN_LENGTH)]
        public string HtmlContent { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
