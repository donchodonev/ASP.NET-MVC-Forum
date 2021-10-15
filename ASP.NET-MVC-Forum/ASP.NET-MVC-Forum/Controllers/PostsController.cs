namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Category;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.DataConstants.PostConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class PostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostService postService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public PostsController(IUserService userService, IPostService postService, ICategoryService categoryService, IMapper mapper)
        {
            this.userService = userService;
            this.postService = postService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ViewPost(int postId, string postTitle)
        {
            var post = await postService.GetByIdAsync(postId, withUserIncluded: true, withIdentityUserIncluded: true, withUserPostsIncluded: true);

            var vm = mapper.Map<ViewPostViewModel>(post);

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var vm = await PrepareAddFormDataOnGetAsync();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm] AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                TempData["Title"] = data.Title;
                TempData["ErrorMessage"] = $"The length of the post must be longer than {HtmlContentMinLength} symbols";
                return RedirectToAction("Add", "Posts");
            }

            if (await postService.PostExistsAsync(data.Title))
            {
                TempData["ErrorMessage"] = $"A post with the title \"{data.Title}\" already exists";
                TempData["HtmlContent"] = data.HtmlContent;
                return RedirectToAction("Add", "Posts");
            }

            var postId = await AddPostAsync(data);

            return RedirectToAction("ViewPost", new { postId = postId, postTitle = data.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            await ValidatePostOwnership(postId);

            var vm = mapper
                .ProjectTo<EditPostFormModel>(await postService.GetByIdAsQueryableAsync(postId, withCategoryIncluded: true))
                .First();

            vm.Categories = await GetCategoryIdAndNameCombinations();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] EditPostFormModel data)
        {
            await ValidatePostOwnership(data.PostId);

            var originalPost = await postService.GetByIdAsync(data.PostId);

            var postChanges = await postService.GetPostChanges(originalPost, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return RedirectToAction("Edit", "Posts", new { postId = data.PostId });
            }

            foreach (var kvp in postChanges)
            {
                if (kvp.Key == "HtmlContent")
                {
                    originalPost.HtmlContent = data.HtmlContent;
                }
                if (kvp.Key == "Title")
                {
                    originalPost.Title = data.Title;
                }
                if (kvp.Key == "CategoryId")
                {
                    originalPost.CategoryId = data.CategoryId;
                }
            }

            var userId = await userService.GetBaseUserIdAsync(this.User.Id());

            await postService.EditPostAsync(originalPost);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int postId)
        {
            await ValidatePostOwnership(postId);

            var isPostDeleted = await postService.IsPostDeleted(postId);

            if (isPostDeleted)
            {
                TempData["ErrorMessage"] = "Your post has already been successfully deleted. Please allow 60 seconds to pass after which will stop being displayed";
                return RedirectToAction("Index", "Home");
            }

            await postService.DeletePostAsync(postId);

            return RedirectToAction("Index","Home");
        }

        private async Task<AddPostFormModel> PrepareAddFormDataOnGetAsync()
        {
            var addPostFormModel = new AddPostFormModel();

            var selectOptions = await GetCategoryIdAndNameCombinations();

            addPostFormModel.Categories = selectOptions;

            if (TempData.ContainsKey("HtmlContent"))
            {
                addPostFormModel.HtmlContent = TempData["HtmlContent"].ToString();
            }
            if (TempData.ContainsKey("Title"))
            {
                addPostFormModel.Title = TempData["Title"].ToString();
            }

            return addPostFormModel;
        }

        private async Task<CategoryIdAndName[]> GetCategoryIdAndNameCombinations()
        {
            var categories = await categoryService.AllAsync();

            var selectOptions = categories
                .ProjectTo<CategoryIdAndName>(mapper.ConfigurationProvider)
                .ToArray();

            return selectOptions;
        }

        private async Task<int> AddPostAsync(AddPostFormModel data)
        {
            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            return await postService.AddPostAsync(newPost, baseUserId);
        }

        private async Task ValidatePostOwnership(int postId)
        {
            var userId = await userService.GetBaseUserIdAsync(this.User.Id());

            if (!await postService.UserCanEditAsync(userId, postId))
            {
                RedirectToAction("Index", "Home");
            }
        }
    }
}
