namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Linq;

    public class CommentQueryBuilder : BaseQueryBuilder<Comment>
    {
        public CommentQueryBuilder(IQueryable<Comment> entities)
            : base(entities)
        {
        }


    }
}
