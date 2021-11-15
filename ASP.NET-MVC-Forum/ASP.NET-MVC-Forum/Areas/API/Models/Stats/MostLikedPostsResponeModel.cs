namespace ASP.NET_MVC_Forum.Areas.API.Models.Stats
{
    public class MostLikedPostsResponeModel : IStatsResponseModel
    {
        public int Count { get; set; }

        public int Id { get; set; }

        public string Color { get; set; }

        public string Title { get; set; }
    }
}
