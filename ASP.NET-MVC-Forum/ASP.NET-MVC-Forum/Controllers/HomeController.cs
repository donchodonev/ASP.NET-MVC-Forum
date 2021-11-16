namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.PostFilterConstants;
    using static ASP.NET_MVC_Forum.Data.DataConstants.PostSortConstants;

    public class HomeController : Controller
    {
        private readonly IPostService postsService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public HomeController(IPostService postsService, ICategoryService categoryService, IMapper mapper)
        {
            this.postsService = postsService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public IActionResult Index(int sortType, int sortOrder, string searchTerm,string category)
        {
            ViewBag.SortTypeLibrary = new SelectList(GetSortOptions(), "Key", "Value", sortType);
            ViewBag.SortOrderOptions = new SelectList(GetOrderType(), "Key", "Value", sortOrder);
            ViewBag.CategoryNames = new SelectList(GetCategories(categoryService), category);
            ViewBag.SearchTerm = searchTerm;

            var posts = GetPosts();

            var sortedPosts = postsService.SortAndOrder(posts, sortType, sortOrder, searchTerm, category);

            return View(sortedPosts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IQueryable<Post> GetPosts()
        {
            return postsService.AllAsync(
                    PostQueryFilter.WithUser,
                    PostQueryFilter.WithIdentityUser,
                    PostQueryFilter.WithoutDeleted,
                    PostQueryFilter.AsNoTracking)
                    .GetAwaiter()
                    .GetResult();
        }
    }
}
