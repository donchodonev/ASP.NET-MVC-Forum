namespace ASP.NET_MVC_Forum.Areas.API.Models
{
    public class CommentPostRequestModel
    {
        public string CommentText { get; set; }

        public int UserId { get; set; }

        public int PostId { get; set; }

        public string Username { get; set; }
    }
}
