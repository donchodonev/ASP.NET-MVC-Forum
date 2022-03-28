namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICategoryRepository
    {
        public IQueryable<T> AllAs<T>(bool includePosts = false);

        public IQueryable<Category> All(bool withPostsIncluded = false);

        public Task<List<string>> GetCategoryNamesAsync();

        public Task<CategoryIdAndNameViewModel[]> GetCategoryIdAndNameCombinationsAsync();
    }
}
