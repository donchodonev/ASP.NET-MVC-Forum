namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly IPostService postsService;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public HomeController(IPostService postsService, IMapper mapper, IMemoryCache memoryCache)
        {
            this.postsService = postsService;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            var vm =
                 mapper.Map<List<PostPreviewViewModel>>(await GetCachedPosts())
                .ToList();

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task<List<Post>> GetCachedPosts()
        {
            return await Task.Run(() => {

                string cacheKey = "allPosts";

                if (!memoryCache.TryGetValue<List<Post>>(cacheKey, out List<Post> posts))
                {
                    posts = postsService
                        .AllAsync(withUserIncluded: true, withIdentityUserIncluded: true)
                        .GetAwaiter()
                        .GetResult()
                        .ToList();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(60),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(50)
                };

                memoryCache.Set(cacheKey, posts, cacheEntryOptions);

                return posts;
            });
        }
    }
}
