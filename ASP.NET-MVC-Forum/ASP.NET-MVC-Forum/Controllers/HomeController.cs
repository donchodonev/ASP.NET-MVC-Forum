﻿namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models;
    using ASP.NET_MVC_Forum.Domain.Models.Post;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.PostSortConstants;
    using static ASP.NET_MVC_Forum.Domain.Constants.PostViewCountOptions;

    public class HomeController : Controller
    {
        private readonly IPostBusinessService postService;
        private readonly ICategoryRepository categoryRepository;

        public HomeController(IPostBusinessService postService,ICategoryRepository categoryRepository)
        {
            this.postService = postService;
            this.categoryRepository = categoryRepository;
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

            await UpdateViewBagAsync(sortType, sortOrder, category, viewCount, searchTerm, personalOnly);

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

        private async Task UpdateViewBagAsync(int sortType, int sortOrder, string category, int viewCount, string searchTerm, string personalOnly)
        {
            ViewBag.SortTypeLibrary = new SelectList(GetSortOptions(), "Key", "Value", sortType);
            ViewBag.SortOrderOptions = new SelectList(GetOrderType(), "Key", "Value", sortOrder);
            ViewBag.CategoryNames = new SelectList((await categoryRepository.GetCategoryNamesAsync()).Prepend("All"), category);
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
