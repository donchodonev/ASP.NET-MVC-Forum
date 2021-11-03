
namespace ASP.NET_MVC_Forum.Areas.API.Models.Comments
{
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Forum.Data.DataConstants.CommentConstants;
    public class CommentPostRequestModel
    {
        [MinLength(ContentMinLength)]
        [MaxLength(ContentMaxLength)]
        [Required]
        public string CommentText { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}
