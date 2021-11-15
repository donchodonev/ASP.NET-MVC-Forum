namespace ASP.NET_MVC_Forum.Services.Category
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext db;

        public CategoryService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IQueryable<Category>> AllAsync(bool withPostsIncluded = false)
        {
            return await Task.Run(() =>
            {
                var query = db.Categories.AsQueryable();

                if (withPostsIncluded)
                {
                    query = query.Include(x => x.Posts);
                }

                return query;
            });
        }

        public List<string> GetCategoryNames()
        {
            return db
                .Categories
                .AsNoTracking()
                .Select(x => x.Name)
                .ToList();
        }
    }
}
