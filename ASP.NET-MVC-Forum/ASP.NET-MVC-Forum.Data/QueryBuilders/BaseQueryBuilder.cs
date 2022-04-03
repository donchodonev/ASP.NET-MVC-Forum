namespace ASP.NET_MVC_Forum.Data.QueryBuilders
{
    using System.Linq;

    public abstract class BaseQueryBuilder<T>
    {
        protected IQueryable<T> entities;

        protected BaseQueryBuilder(IQueryable<T> entities)
        {
            this.entities = entities;
        }

        public IQueryable<T> BuildQuery()
        {
            return entities;
        }
    }
}
