namespace ASP.NET_MVC_Forum.Web.Services.Data.Category
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Web.Models.Post;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICategoryDataService
    {
        public IQueryable<Category> All(bool withPostsIncluded = false);
        public List<string> GetCategoryNames();

        public CategoryIdAndNameViewModel[] GetCategoryIdAndNameCombinations();
    }
}
