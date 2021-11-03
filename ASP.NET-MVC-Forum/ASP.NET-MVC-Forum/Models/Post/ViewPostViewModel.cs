namespace ASP.NET_MVC_Forum.Models.Post
{
    public class ViewPostViewModel
    {
        public ViewPostViewModel()
        {

        }

        public string Title { get; set; }

        public string UserImageUrl { get; set; }

        public int UserPostsCount { get; set; }

        public string HtmlContent { get; set; }

        public string UserMemberSince { get; set; }

        public string UserUsername { get; set; }

        public string PostCreationDate { get; set; }

        public string UserIdentityUserUsername { get; set; }

        public int PostId { get; set; }

        public int CommentsCount { get; set; }

        public int VoteSum { get; set; }
    }
}
