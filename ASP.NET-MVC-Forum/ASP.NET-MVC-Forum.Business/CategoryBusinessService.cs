namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;

    using System.Collections.Generic;
    using System.Linq;

    public class CategoryBusinessService : ICategoryBusinessService
    {
        private readonly ICategoryDataService data;

        public CategoryBusinessService(ICategoryDataService data)
        {
            this.data = data;
        }
        public IReadOnlyCollection<string> GetCategories()
        {
            return data
                .GetCategoryNames()
                .Prepend("All")
                .ToList()
                .AsReadOnly();
        }
    }
}
