namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Category;
    using ASP.NET_MVC_Forum.Services.Category;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IMemoryCache memoryCache;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categoryService, IMemoryCache memoryCache, IMapper mapper)
        {
            this.categoryService = categoryService;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
        }

        public IActionResult All(string searchTerm)
        {
            var categories = mapper.Map<List<AllCategoryViewModel>>(GetCachedCategories());

            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = GetSearchedCategories(categories, searchTerm);
            }

            return View(categories);
        }

        public IActionResult CategoryContent([FromQuery] CategoryContentCategoryInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }

            return View();
        }

        private List<Category> GetCachedCategories()
        {
            string cacheKey = "allCategories";

            if (!memoryCache.TryGetValue<List<Category>>(cacheKey, out List<Category> categoryList))
            {
                categoryList = categoryService
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

        private List<AllCategoryViewModel> GetSearchedCategories(List<AllCategoryViewModel> categories, string searchTerm)
        {
            categories = categories.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

            return categories;
        }
    }
}
