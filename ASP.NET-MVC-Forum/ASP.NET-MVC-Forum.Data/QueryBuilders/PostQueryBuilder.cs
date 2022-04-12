namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Enums;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;

    public class PostQueryBuilder : BaseQueryBuilder<Post>
    {
        public PostQueryBuilder(IQueryable<Post> entities)
            : base(entities)
        {
        }

        public PostQueryBuilder WithoutDeleted()
        {
            entities = entities.Where(x => !x.IsDeleted);

            return this;
        }

        public PostQueryBuilder IncludeComments()
        {
            entities = entities.Include(x => x.Comments);

            return this;
        }

        public PostQueryBuilder IncludeCategories()
        {
            entities = entities.Include(x => x.Category);

            return this;
        }

        public PostQueryBuilder IncludeVotes()
        {
            entities = entities.Include(x => x.Votes);

            return this;
        }

        public PostQueryBuilder IncludeReports()
        {
            entities = entities.Include(x => x.Reports);

            return this;
        }

        public PostQueryBuilder IncludeUser()
        {
            entities = entities.Include(x => x.User);

            return this;
        }

        public PostQueryBuilder Order(int orderType, int orderDirection)
        {
            entities = orderDirection switch
            {
                1 => OrderAscendingBy(orderType),
                0 => OrderDescendingBy(orderType),
                _ => entities
            };

            return this;
        }

        public PostQueryBuilder FindBySearchTerm(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                entities = entities.Where(post => post.Title.Contains(searchTerm));
            }

            return this;
        }

        public PostQueryBuilder FindByCategoryName(string category)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrWhiteSpace(category) && category != "All")
            {
                entities = entities.Where(post => post.Category.Name == category);
            }

            return this;
        }

        private IQueryable<Post> OrderAscendingBy(int orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                0 => entities.OrderBy(x => x.CreatedOn).ThenBy(x => x.ModifiedOn),
                1 => entities.OrderBy(x => x.Title),
                _ => entities
            };

            return query;
        }

        private IQueryable<Post> OrderDescendingBy(int orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                0 => entities.OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.ModifiedOn),
                1 => entities.OrderByDescending(x => x.Title),
                _ => entities
            };

            return query;
        }
    }
}
