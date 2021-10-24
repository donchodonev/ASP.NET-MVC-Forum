namespace ASP.NET_MVC_Forum.Services.Comment.Models
{
    public class RawCommentServiceModel
    {
        public int PostId { get; set; }

        public string Username { get; set; }

        public string CommentText { get; set; }

        public int UserId { get; set; }
    }
}
