namespace ASP.NET_MVC_Forum.Data.Constants
{
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using System.Collections.Generic;
    using System.Linq;

    public class PostFilterConstants
    {
        /// <summary>
        /// Returns all existing post categories from the database PLUS the fictional category "All" prepended as the zero-index element
        /// </summary>
        /// <param name="categoryService"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<string> GetCategories(ICategoryService categoryService)
        {
            return categoryService
                .GetCategoryNames()
                .Prepend("All")
                .ToList()
                .AsReadOnly();
        }
    }
}
