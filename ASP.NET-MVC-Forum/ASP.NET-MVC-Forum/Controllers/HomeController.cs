﻿namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants.PostFilterConstants;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants.PostSortConstants;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants.PostViewCountOptions;

    public class HomeController : Controller
    {
        private readonly IPostDataService postDataService;
        private readonly IPostBusinessService postBusinessService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public HomeController(IPostDataService postDataService ,IPostBusinessService postBusinessService, ICategoryService categoryService, IMapper mapper)
        {
            this.postDataService = postDataService;
            this.postBusinessService = postBusinessService;
            this.categoryService = categoryService;
            this.mapper = mapper;
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

            var sortedPosts = postBusinessService
                .SortAndOrder(GetPosts(), sortType, sortOrder, searchTerm, category)
                .ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider);

            var paginatedList = await PaginatedList<PostPreviewViewModel>
                .CreateAsync(sortedPosts, pageNumber, viewCount);

            return View(paginatedList);
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
            return postDataService.All(
                    PostQueryFilter.WithUser,
                    PostQueryFilter.WithIdentityUser,
                    PostQueryFilter.WithoutDeleted,
                    PostQueryFilter.AsNoTracking);
        }

        private void UpdateViewBag(int sortType, int sortOrder, string category, int viewCount, string searchTerm, string personalOnly)
        {
            ViewBag.SortTypeLibrary = new SelectList(GetSortOptions(), "Key", "Value", sortType);
            ViewBag.SortOrderOptions = new SelectList(GetOrderType(), "Key", "Value", sortOrder);
            ViewBag.CategoryNames = new SelectList(GetCategories(categoryService), category);
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
