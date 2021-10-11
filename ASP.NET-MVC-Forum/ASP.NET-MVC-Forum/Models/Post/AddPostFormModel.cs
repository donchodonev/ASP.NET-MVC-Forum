using ASP.NET_MVC_Forum.Models.Category;

namespace ASP.NET_MVC_Forum.Models.Post
{
    public class AddPostFormModel
    {
        public  CategoryIdAndName[] Categories { get; set; }

        public string HtmlContent { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }
    }
}
