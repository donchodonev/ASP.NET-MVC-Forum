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

    public class HomeController : Controller
    {
        private readonly IPostService postsService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private Dictionary<int, string> sortTypeLibrary;
        private Dictionary<int, string> sortOrderOptions;
        private IReadOnlyCollection<string> postCategoryNames;

        public HomeController(IPostService postsService, ICategoryService categoryService, IMapper mapper)
        {
            this.postsService = postsService;
            this.categoryService = categoryService;
            this.mapper = mapper;

            sortTypeLibrary = GetSortOptions();
            sortOrderOptions = GetOrderOptions();
            postCategoryNames = categoryService.GetCategoryNames().Prepend("All").ToList();
        }

        public IActionResult Index(int sortType, int sortOrder, string searchTerm,string category)
        {
            ViewBag.SortTypeLibrary = new SelectList(sortTypeLibrary, "Key", "Value", sortType);
            ViewBag.SortOrderOptions = new SelectList(sortOrderOptions, "Key", "Value", sortOrder);
            ViewBag.CategoryNames = new SelectList(postCategoryNames, category);
            ViewBag.SearchTerm = searchTerm;

            var posts = GetPosts();

            var sortedPosts = SortAndOrder(posts, sortType, sortOrder, searchTerm, category);

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

        private Dictionary<int, string> GetSortOptions()
        {
            return new Dictionary<int, string>()
            {
                {0,"Date" },
                { 1,"Post Title"}
            };
        }

        private Dictionary<int, string> GetOrderOptions()
        {
            return new Dictionary<int, string>()
            {
                {0,"Ascending" },
                { 1,"Descending"}
            };
        }

        private List<PostPreviewViewModel> SortAndOrder(IQueryable<Post> posts, int sortType, int sortOrder, string searchTerm, string category)
        {
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                posts = posts.Where(post => post.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category) && !string.IsNullOrWhiteSpace(category) && category != "All")
            {
                posts = posts.Where(post => post.Category.Name == category);
            }


            if (sortOrder == 0 && sortType == 0)
            {
                posts = posts.OrderBy(x => x.CreatedOn);
            }
            else if (sortOrder == 0 && sortType == 1)
            {
                posts = posts.OrderBy(x => x.Title);
            }
            else if (sortOrder == 1 && sortType == 0)
            {
                posts = posts.OrderByDescending(x => x.CreatedOn);
            }
            else if (sortOrder == 1 && sortType == 1)
            {
                posts = posts.OrderByDescending(x => x.Title);
            }

            return posts
                .ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider)
                .ToList();
        }
    }
}
