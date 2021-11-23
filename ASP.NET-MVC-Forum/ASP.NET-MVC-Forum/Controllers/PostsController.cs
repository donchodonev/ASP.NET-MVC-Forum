namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.DataConstants.PostConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private const string PostDeletedMessege = "Your post has already been successfully deleted. Please allow 60 seconds to pass after which will stop being displayed";
        private const string ReportThankYouMessage = "Thank you for your report, our moderators will review it as quickly as possible !";

        private readonly IUserService userService;
        private readonly IPostService postService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly SignInManager<IdentityUser> signInManager;

        public PostsController(
            IUserService userService,
            IPostService postService,
            ICategoryService categoryService,
            IMapper mapper,
            SignInManager<IdentityUser> signInManager)
        {
            this.userService = userService;
            this.postService = postService;
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postService.GetByIdAsync(postId,
                PostQueryFilter.WithIdentityUser,
                PostQueryFilter.WithUserPosts,
                PostQueryFilter.WithComments,
                PostQueryFilter.WithVotes);

            if (post == null)
            {
                return BadRequest();
            }

            var vm = mapper.Map<ViewPostViewModel>(post);

            if (signInManager.IsSignedIn(this.User))
            {
                vm.UserLastVoteChoice = await GetUserLastVote(post, vm, this.User);
            }

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
            await ValidatePostOwnership(postId, this.User);

            var post = await postService
                .GetByIdAsQueryableAsync(postId, PostQueryFilter.WithCategory);

            var vm = mapper
                .ProjectTo<EditPostFormModel>(post)
                .First();

            vm.Categories = await GetCategoryIdAndNameCombinations();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] EditPostFormModel data)
        {
            await ValidatePostOwnership(data.PostId, this.User);

            var originalPost = await postService.GetByIdAsync(data.PostId);

            var postChanges = postService.GetPostChanges(originalPost, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return RedirectToAction("Edit", "Posts", new { postId = data.PostId });
            }

            AddPostChanges(originalPost, data, postChanges);

            await postService.EditPostAsync(originalPost);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            await ValidatePostOwnership(postId, this.User);

            var isPostDeleted = await postService.IsPostDeleted(postId, postTitle);

            if (isPostDeleted.Value == true)
            {
                this.RedirectToActionWithErrorMessage(PostDeletedMessege, "Index", "Home");
            }
            else if (!isPostDeleted.HasValue)
            {
                BadRequest();
            }

            await postService.DeletePostAsync(postId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postService.AddPostReport(postId, content);

            return this.RedirectToActionWithMessage(ReportThankYouMessage, "Home", "Index");
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

        private async Task<CategoryIdAndNameViewModel[]> GetCategoryIdAndNameCombinations()
        {
            var categories = await categoryService.AllAsync();

            var selectOptions = categories
                .ProjectTo<CategoryIdAndNameViewModel>(mapper.ConfigurationProvider)
                .ToArray();

            return selectOptions;
        }

        private async Task<int> AddPostAsync(AddPostFormModel data)
        {
            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            return await postService.AddPostAsync(newPost, baseUserId);
        }

        private async Task ValidatePostOwnership(int postId, ClaimsPrincipal principal)
        {
            var userId = await userService.GetBaseUserIdAsync(User.Id());

            if (!await postService.UserCanEditAsync(userId, postId, principal))
            {
                RedirectToAction("Index", "Home");
            }
        }

        private async Task<int> GetUserLastVote(Post post, ViewPostViewModel vm, ClaimsPrincipal user)
        {
            int userId = await userService
                .GetBaseUserIdAsync(user.Id());


            var userLastVote = post.Votes.FirstOrDefault(x => x.UserId == userId);

            if (userLastVote != null)
            {
                return (int)userLastVote.VoteType;
            }

            return 0;
        }

        private void AddPostChanges(Post originalPost, EditPostFormModel newPostData, Dictionary<string, bool> postChanges)
        {
            foreach (var kvp in postChanges)
            {
                if (kvp.Key == "HtmlContent")
                {
                    originalPost.HtmlContent = newPostData.HtmlContent;
                }
                if (kvp.Key == "Title")
                {
                    originalPost.Title = newPostData.Title;
                }
                if (kvp.Key == "CategoryId")
                {
                    originalPost.CategoryId = newPostData.CategoryId;
                }
            }
        }
    }
}
