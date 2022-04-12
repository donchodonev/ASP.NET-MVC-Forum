namespace ASP.NET_MVC_Forum.Domain.Models.Post
{
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.PostConstants;
    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class EditPostFormModel
    {
        public CategoryIdAndNameViewModel[] Categories { get; set; }

        [Required(ErrorMessage = TITLE_TOO_SHORT)]
        [MinLength(TITLE_MIN_LENGTH, ErrorMessage = TITLE_TOO_SHORT)]
        [MaxLength(TITLE_MAX_LENGTH, ErrorMessage = TITLE_TOO_LONG)]
        public string Title { get; set; }

        [Required(ErrorMessage = POST_LENGTH_TOO_SMALL)]
        [MinLength(HTML_CONTENT_MIN_LENGTH, ErrorMessage = POST_LENGTH_TOO_SMALL)]
        public string HtmlContent { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CategoryId { get; set; }

        public int PostId { get; set; }
    }
}
