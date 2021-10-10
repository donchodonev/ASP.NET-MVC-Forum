namespace ASP.NET_MVC_Forum.Services.Category
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        public Task<IQueryable<Category>> AllAsync(bool withPostsIncluded = false);
    }
}
