namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Enums;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostQueryBuilder
    {
        private IQueryable<Post> posts;

        public PostQueryBuilder(IQueryable<Post> posts)
        {
            this.posts = posts;
        }

        public PostQueryBuilder WithoutDeleted()
        {
            posts = posts.Where(x => !x.IsDeleted);

            return this;
        }

        public PostQueryBuilder IncludeComments()
        {
            posts = posts.Include(x => x.Comments);

            return this;
        }

        public PostQueryBuilder IncludeCategories()
        {
            posts = posts.Include(x => x.Category);

            return this;
        }

        public PostQueryBuilder IncludeVotes()
        {
            posts = posts.Include(x => x.Votes);

            return this;
        }

        public PostQueryBuilder IncludeReports()
        {
            posts = posts.Include(x => x.Reports);

            return this;
        }

        public PostQueryBuilder Order(OrderDirection direction, PostOrderType orderType)
        {
            posts = direction switch
            {
                OrderDirection.Ascending => OrderAscendingBy(orderType),
                OrderDirection.Descending => OrderDescendingBy(orderType),
                _ => posts
            };

            return this;
        }

        public IQueryable<Post> BuildQuery()
        {
            return posts;
        }

        public Task<List<T>> GetAllAsyncAs<T>(IMapper mapper)
        {
            return posts.ProjectTo<T>(mapper.ConfigurationProvider).ToListAsync();
        }

        private IQueryable<Post> OrderAscendingBy(PostOrderType orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                PostOrderType.CommentCount => posts.OrderBy(x => x.Comments.Count),
                PostOrderType.VoteTypeSum => posts.OrderBy(x => x.Votes.Sum(x => (int)x.VoteType)),
                PostOrderType.ReportsCount => posts.OrderBy(x => x.Reports.Count),
                _ => posts
            };

            return query;
        }

        private IQueryable<Post> OrderDescendingBy(PostOrderType orderType)
        {
            IQueryable<Post> query;

            query = orderType switch
            {
                PostOrderType.CommentCount => posts.OrderByDescending(x => x.Comments.Count),
                PostOrderType.VoteTypeSum => posts.OrderByDescending(x => x.Votes.Sum(x => (int)x.VoteType)),
                PostOrderType.ReportsCount => posts.OrderByDescending(x => x.Reports.Count),
                _ => posts
            };

            return query;
        }
    }
}
