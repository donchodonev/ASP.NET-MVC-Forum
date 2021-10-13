﻿namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Category;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
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
        private readonly IPostService postService;
        private readonly IMemoryCache memoryCache;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryService categoryService,IPostService postService, IMemoryCache memoryCache, IMapper mapper)
        {
            this.categoryService = categoryService;
            this.postService = postService;
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

        public async Task<IActionResult> CategoryContent([FromRoute] int categoryId, string categoryName)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest();
            }

            var vm =
                mapper
                .Map<List<PostPreviewViewModel>>(postService.GetByCategoryAsync(categoryId, withUserIncluded:true, withIdentityUserIncluded:true)
                .GetAwaiter()
                .GetResult()
                .ToList());

            return View(vm);
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