namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;

    public class CategoryQueryBuilder
    {
        private IQueryable<Category> categories;

        public CategoryQueryBuilder(IQueryable<Category> categories)
        {
            this.categories = categories;
        }

        public CategoryQueryBuilder WithoutDeleted()
        {
            categories = categories.Where(x => !x.IsDeleted);

            return this;
        }

        public CategoryQueryBuilder IncludePosts()
        {
            categories = categories.Include(x => x.Posts);

            return this;
        }

        public IQueryable<Category> BuildQuery()
        {
            return categories;
        }
    }
}
