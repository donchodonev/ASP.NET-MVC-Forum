namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Category.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoriesController :  Controller
    {
        private readonly ICategoryService categories;
        private readonly IMemoryCache memoryCache;

        public CategoriesController(ICategoryService categories, IMemoryCache memoryCache)
        {
            this.categories = categories;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> All()
        {
            return Json(GetCachedCategories());
        }

        private List<Category> GetCachedCategories()
        {
            string cacheKey = "allCategories";

            if (!memoryCache.TryGetValue<List<Category>>(cacheKey, out List<Category> categoryList))
            {
                categoryList = categories
                    .AllAsync()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();
            }

            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddSeconds(60),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(50)
            };

            return categoryList;
        }
    }
}
