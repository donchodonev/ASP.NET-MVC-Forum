namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;

    public class CommentReportQueryBuilder : BaseQueryBuilder<CommentReport>
    {
        public CommentReportQueryBuilder(IQueryable<CommentReport> entities) 
            : base(entities)
        {
        }

        public CommentReportQueryBuilder WithoutDeleted()
        {
            entities = entities.Where(x => !x.IsDeleted);

            return this;
        }

        public CommentReportQueryBuilder DeletedOnly()
        {
            entities = entities.Where(x => x.IsDeleted);

            return this;
        }

        public CommentReportQueryBuilder IncludeComment()
        {
            entities = entities.Include(x => x.Comment);

            return this;
        }
    }
}
