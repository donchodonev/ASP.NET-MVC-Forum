using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MVC_Forum.Models.Post
{
    using static ASP.NET_MVC_Forum.Data.DataConstants.DateTimeFormat;
    public class PostPreviewViewModel
    {
        public string Title { get; set; }

        public string HtmlContent { get; set; }

        public string ShortDescription { get; set; }

        public string UserImageUrl { get; set; }

        public int PostsCount { get; set; }

        public string MemberSince { get; set; }

        public string Username { get; set; }

        public string PostCreationDate { get; set; }

        public string UserIdentityUserUsername { get; set; }
    }
}
