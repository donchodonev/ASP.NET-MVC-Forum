namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models;
    using ASP.NET_MVC_Forum.Services.Comment;
    using ASP.NET_MVC_Forum.Services.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [ApiController]
    [Area("API")]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    public class CommentsController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ICommentService commentService;

        public CommentsController(IUserService userService, ICommentService commentService)
        {
            this.userService = userService;
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IEnumerable<CommentGetRequestResponseModel>> GetComments(int postId)
        {
            return await commentService.AllComments(postId);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentPostRequestModel>> AddComment(CommentPostRequestModel commentData)
        {
            var userId = await userService.GetBaseUserIdAsync(this.User.Id());

            commentData.UserId = userId;

            await commentService.AddComment(commentData);

            return Ok(commentData);
        }
    }
}

