namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Category;
    using ASP.NET_MVC_Forum.Services.Category.Contracts;
    using AutoMapper;
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
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categories, IMemoryCache memoryCache, IMapper mapper)
        {
            this.categories = categories;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
        }

        public async Task<IActionResult> All()
        {
            var categories = mapper.Map<List<AllCategoryViewModel>>(GetCachedCategories());

            return View(categories);
        }

        public IActionResult CategoryContent()
        {
            return View();
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
