namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.Category;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.PostSortConstants;
    using static ASP.NET_MVC_Forum.Data.Constants.PostViewCountOptions;

    public class HomeController : Controller
    {
        private readonly IPostBusinessService postService;
        private readonly ICategoryBusinessService categoryService;

        public HomeController(IPostBusinessService postService,ICategoryBusinessService categoryService)
        {
            this.postService = postService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(
            int sortType,
            int sortOrder,
            string searchTerm,
            string category,
            int viewCount,
            string personalOnly,
            int pageNumber = 1)
        {

            UpdateViewBag(sortType, sortOrder, category, viewCount, searchTerm, personalOnly);

            var sortedPosts = postService
                .GetAllPostsSortedBy(sortType, sortOrder, searchTerm, category);

            return View(await PaginatedList<PostPreviewViewModel>.CreateAsync(sortedPosts, pageNumber, viewCount));
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

        private void UpdateViewBag(int sortType, int sortOrder, string category, int viewCount, string searchTerm, string personalOnly)
        {
            ViewBag.SortTypeLibrary = new SelectList(GetSortOptions(), "Key", "Value", sortType);
            ViewBag.SortOrderOptions = new SelectList(GetOrderType(), "Key", "Value", sortOrder);
            ViewBag.CategoryNames = new SelectList(categoryService.GetCategories(), category);
            ViewBag.ViewCountOptions = new SelectList(GetViewCountOptions(), viewCount);

            if (personalOnly == "checked")
            {
                ViewBag.PersonalOnly = "checked";
            }

            ViewBag.PersonalOnly = "";
            ViewBag.SearchTerm = searchTerm;
        }
    }
}
