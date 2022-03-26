namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using System.Collections.Generic;

    public interface ICategoryBusinessService
    {
        public IReadOnlyCollection<string> GetCategories();
    }
}
