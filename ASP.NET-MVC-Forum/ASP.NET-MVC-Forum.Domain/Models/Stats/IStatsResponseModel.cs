namespace ASP.NET_MVC_Forum.Domain.Models.Stats
{
    public interface IStatsResponseModel
    {
        public int Count { get; set; }

        public int Id { get; set; }

        public string Color { get; set; }

        public string Title { get; set; }
    }
}
