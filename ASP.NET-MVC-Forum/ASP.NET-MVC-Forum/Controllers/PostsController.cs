namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

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

            if (post == null)
            {
                return this.RedirectToActionWithErrorMessage(Error.POST_DOES_NOT_EXIST, "Home", "Index");
            }

            if (signInManager.IsSignedIn(this.User))
            {
                await postService.InjectUserLastVoteType(post, this.User.Id());
            }

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var addPostFormModel = await postService.GeneratedAddPostFormModelAsync();

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
                return this.RedirectToActionWithErrorMessage(Error.POST_LENGTH_TOO_SMALL, "Posts", "Add");
            }

            if (await postService.PostExistsAsync(data.Title))
            {
                TempData["Title"] = data.Title;
                TempData["HtmlContent"] = data.HtmlContent;

                return this.RedirectToActionWithErrorMessage(Error.DUPLICATE_POST_NAME, "Posts", "Add");
            }

            var newlyCreatedPost = await postService.CreateNewAsync(data);

            return RedirectToAction("ViewPost", new { postId = newlyCreatedPost.Id, postTitle = newlyCreatedPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            if (!await postService.IsUserPrivileged(postId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YOU_ARE_NOT_THE_AUTHER, "Home", "Index");
            }

            var vm = await postService.GenerateEditPostFormModelAsync(postId);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditPostFormModel data)
        {
            if (!await postService.IsUserPrivileged(data.PostId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YOU_ARE_NOT_THE_AUTHER, "Home", "Index");
            }

            var postChanges = await postService
                .GetPostChangesAsync(data.PostId, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(Error.POST_IS_UNCHANGED, "Posts", "Edit", new { postId = data.PostId });
            }

            await postService.Edit(data);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            if (!await postService.IsUserPrivileged(postId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YOU_ARE_NOT_THE_AUTHER, "Home", "Index");
            }

            if (!await postService.PostExistsAsync(postId))
            {
                BadRequest();
            }

            await postService.Delete(postId);

            return this.RedirectToActionWithSuccessMessage(Success.POST_DELETED, "Home", "Index");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postReportService.ReportAsync(postId, content);

            return this.RedirectToActionWithSuccessMessage(Success.REPORT_THANK_YOU_MESSAGE, "Home", "Index");
        }
    }
}
