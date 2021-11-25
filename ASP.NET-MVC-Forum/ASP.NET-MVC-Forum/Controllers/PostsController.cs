namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.Data.Vote;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Success;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostDataService postDataService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPostBusinessService postBusinessService;
        private readonly IPostReportBusinessService postReportBusinessService;
        private readonly IVoteDataService voteDataService;

        public PostsController(
            IUserService userService,
            IPostDataService postDataService,
            ICategoryService categoryService,
            IMapper mapper,
            SignInManager<IdentityUser> signInManager,
            IPostBusinessService postBusinessService,
            IPostReportBusinessService postReportBusinessService,
            IVoteDataService voteDataService)
        {
            this.userService = userService;
            this.postDataService = postDataService;
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.postBusinessService = postBusinessService;
            this.postReportBusinessService = postReportBusinessService;
            this.voteDataService = voteDataService;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postBusinessService.GenerateViewPostModel(postId);

            if (post == null)
            {
                return this.RedirectToActionWithErrorMessage(SuchAPostDoesNotExist, "Home", "Index");
            }

            if (signInManager.IsSignedIn(this.User))
            {
                await postBusinessService.InjectUserLastVoteType(post, this.User.Id());
            }

            return View(post);
        }

        [Authorize]
        public IActionResult Add()
        {
            var addPostFormModel = new AddPostFormModel();

            addPostFormModel.FillCategories(categoryService);

            if (TempData.ContainsKey("HtmlContent"))
            {
                addPostFormModel.HtmlContent = TempData["HtmlContent"].ToString();
            }
            if (TempData.ContainsKey("Title"))
            {
                addPostFormModel.Title = TempData["Title"].ToString();
            }

            return View(addPostFormModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm] AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                TempData["Title"] = data.Title;
                return this.RedirectToActionWithErrorMessage(PostLengthTooSmall, "Posts", "Add");
            }

            if (await postDataService.PostExistsAsync(data.Title))
            {
                TempData["HtmlContent"] = data.HtmlContent;

                var errorMessage = $"A post with the title \"{data.Title}\" already exists";

                return this.RedirectToActionWithErrorMessage(errorMessage, "Posts", "Add");
            }

            var newPost = await AddPostAsync(data);

            await postReportBusinessService
                .AutoGeneratePostReportAsync(newPost.Title, newPost.HtmlContent, newPost.Id);

            return RedirectToAction("ViewPost", new { postId = newPost.Id, postTitle = newPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            var verificationResult = await IsUserPrivileged(postId);

            if (verificationResult != null)
            {
                return this.RedirectToActionWithErrorMessage(YouAreNotTheAuthor, "Home", "Index");
            }

            var post = postDataService
                .GetByIdAsQueryable(postId, PostQueryFilter.WithCategory);

            var vm = mapper
                .ProjectTo<EditPostFormModel>(post)
                .First();

            vm.FillCategories(categoryService);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditPostFormModel data)
        {
            var verificationResult = await IsUserPrivileged(data.PostId);

            if (verificationResult != null)
            {
                return verificationResult;
            }

            var originalPost = await postDataService.GetByIdAsync(data.PostId);

            var postChanges = postBusinessService.GetPostChanges(originalPost, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return RedirectToAction("Edit", "Posts", new { postId = data.PostId });
            }

            AddPostChanges(originalPost, data, postChanges);

            await postBusinessService.Edit(originalPost);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            var verificationResult = await IsUserPrivileged(postId);

            if (verificationResult != null)
            {
                return verificationResult;
            }

            var isPostDeleted = await postDataService.IsPostDeleted(postId, postTitle);

            if (!isPostDeleted.HasValue)
            {
                BadRequest();
            }

            await postBusinessService.Delete(postId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postDataService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postReportBusinessService.ReportAsync(postId, content);

            return this.RedirectToActionWithSuccessMessage(ReportThankYouMessage, "Home", "Index");
        }

        private async Task<Post> AddPostAsync(AddPostFormModel data)
        {
            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            await postBusinessService.CreateNew(newPost, baseUserId);

            return newPost;
        }

        private async Task<ActionResult> IsUserPrivileged(int postId)
        {
            var userId = await userService.GetBaseUserIdAsync(User.Id());

            if (!await postBusinessService.IsAuthor(userId, postId) && !this.User.IsAdminOrModerator())
            {
                return this.RedirectToActionWithErrorMessage(YouAreNotTheAuthor, "Home", "Index");
            }

            return null;
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
