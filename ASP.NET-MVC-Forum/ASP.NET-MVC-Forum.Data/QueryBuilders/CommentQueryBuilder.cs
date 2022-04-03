namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;

    public class CommentQueryBuilder : BaseQueryBuilder<Comment>
    {
        public CommentQueryBuilder(IQueryable<Comment> entities)
            : base(entities)
        {
        }

        public CommentQueryBuilder WithoutDeleted()
        {
            entities = entities.Where(x => !x.IsDeleted);

            return this;
        }

        public CommentQueryBuilder IncludeBaseUser()
        {
            entities = entities.Include(x => x.User);

            return this;
        }

        public CommentQueryBuilder IncludeBaseAndIdentityUser()
        {
            entities = entities
                .Include(x => x.User)
                .ThenInclude(x => x.IdentityUser);

            return this;
        }
    }
}
