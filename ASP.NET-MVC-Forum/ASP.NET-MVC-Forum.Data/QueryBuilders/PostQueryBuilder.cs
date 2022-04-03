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

        public PostQueryBuilder Order(OrderDirection direction, PostOrderType orderType)
        {
            entities = direction switch
            {
                OrderDirection.Ascending => OrderAscendingBy(orderType),
                OrderDirection.Descending => OrderDescendingBy(orderType),
                _ => entities
            };

            return this;
        }

        private IQueryable<Post> OrderAscendingBy(PostOrderType orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                PostOrderType.CommentCount => entities.OrderBy(x => x.Comments.Count),
                PostOrderType.VoteTypeSum => entities.OrderBy(x => x.Votes.Sum(x => (int)x.VoteType)),
                PostOrderType.ReportsCount => entities.OrderBy(x => x.Reports.Count),
                _ => entities
            };

            return query;
        }

        private IQueryable<Post> OrderDescendingBy(PostOrderType orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                PostOrderType.CommentCount => entities.OrderByDescending(x => x.Comments.Count),
                PostOrderType.VoteTypeSum => entities.OrderByDescending(x => x.Votes.Sum(x => (int)x.VoteType)),
                PostOrderType.ReportsCount => entities.OrderByDescending(x => x.Reports.Count),
                _ => entities
            };

            return query;
        }
    }
}
