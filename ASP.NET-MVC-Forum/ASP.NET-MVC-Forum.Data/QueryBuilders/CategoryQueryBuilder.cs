namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;

    public class CategoryQueryBuilder : BaseQueryBuilder<Category>
    {
        public CategoryQueryBuilder(IQueryable<Category> entities) 
            : base(entities)
        {
        }

        public CategoryQueryBuilder WithoutDeleted()
        {
            entities = entities.Where(x => !x.IsDeleted);

            return this;
        }

        public CategoryQueryBuilder IncludePosts()
        {
            entities = entities.Include(x => x.Posts);

            return this;
        }
    }
}
