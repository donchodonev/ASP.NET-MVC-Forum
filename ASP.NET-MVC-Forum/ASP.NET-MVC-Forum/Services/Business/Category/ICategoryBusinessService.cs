namespace ASP.NET_MVC_Forum.Services.Business.Category
{
    using System.Collections.Generic;

    public interface ICategoryBusinessService
    {
        public IReadOnlyCollection<string> GetCategories();
    }
}
