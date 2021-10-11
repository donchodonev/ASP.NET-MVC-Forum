namespace ASP.NET_MVC_Forum.Models.Post
{
    public class PostViewModel
    {
        public string Title { get; set; }

        public string HtmlContent { get; set; }

        public string ImageUrl { get; set; }

        public string UserImageUrl { get; set; }

        public int PostsCount { get; set; }

        public string UserIdentityUserUsername { get; set; }
    }
}
