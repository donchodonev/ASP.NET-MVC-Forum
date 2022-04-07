namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private readonly SignInManager<ExtendedIdentityUser> signInManager;
        private readonly IPostService postService;
        private readonly IPostReportService postReportService;

        public PostsController(
            SignInManager<ExtendedIdentityUser> signInManager,
            IPostService postService,
            IPostReportService postReportService)
        {
            this.signInManager = signInManager;
            this.postService = postService;
            this.postReportService = postReportService;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postService.GenerateViewPostModelAsync(postId);

            if (signInManager.IsSignedIn(this.User))
            {
                await postService.InjectUserLastVoteType(post, this.User.Id());
            }

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Add(string title, string htmlContent, int categoryId)
        {
            var addPostFormModel = await postService.GeneratedAddPostFormModelAsync();

            addPostFormModel.Title = title;
            addPostFormModel.HtmlContent = htmlContent;
            addPostFormModel.CategoryId = categoryId;

            return View(addPostFormModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm] AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                return this.RedirectToActionWithErrorMessage(
                    Error.POST_LENGTH_TOO_SMALL,
                    "Posts",
                    "Add",
                    new
                    {
                        title = data.Title,
                        htmlContent = data.HtmlContent,
                        categoryId = data.CategoryId
                    });
            }

            var newlyCreatedPost = await postService.CreateNewAsync(data,this.User.Id());

            return RedirectToAction("ViewPost", new { postId = newlyCreatedPost.Id, postTitle = newlyCreatedPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            var vm = await postService.GenerateEditPostFormModelAsync(postId, this.User);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditPostFormModel data)
        {
            await postService.Edit(data,this.User);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            await postService.Delete(postId, this.User);

            return this.RedirectToActionWithSuccessMessage(Success.POST_DELETED, "Home", "Index");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            await postReportService.ReportAsync(postId, content);

            return this.RedirectToActionWithSuccessMessage(Success.REPORT_THANK_YOU_MESSAGE, "Home", "Index");
        }
    }
}
