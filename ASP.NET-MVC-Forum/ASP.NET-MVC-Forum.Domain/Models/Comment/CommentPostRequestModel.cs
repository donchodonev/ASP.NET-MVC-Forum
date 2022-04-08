
namespace ASP.NET_MVC_Forum.Domain.Models.Comment
{
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.CommentConstants;
    public class CommentPostRequestModel
    {
        [MinLength(CONTENT_MIN_LENGTH)]
        [MaxLength(CONTENT_MAX_LENGTH)]
        [Required]
        public string CommentText { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}
