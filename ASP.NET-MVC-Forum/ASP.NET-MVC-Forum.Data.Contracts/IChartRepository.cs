namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Linq;

    public interface IChartRepository
    {
        public IQueryable<T> GetStatsAs<T>(int count, IQueryable<Post> posts);

        public IQueryable<T> GetStatsAs<T>(int count, IQueryable<Category> categories);
    }
}
