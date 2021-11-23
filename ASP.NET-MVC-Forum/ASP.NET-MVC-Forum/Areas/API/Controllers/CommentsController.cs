namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
    using ASP.NET_MVC_Forum.Services.Comment;
    using ASP.NET_MVC_Forum.Services.Comment.Models;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ICommentService commentService;
        private readonly IMapper mapper;

        public CommentsController(IUserService userService, ICommentService commentService, IMapper mapper)
        {
            this.userService = userService;
            this.commentService = commentService;
            this.mapper = mapper;
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
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var rawCommentData = mapper.Map<RawCommentServiceModel>(commentData);

            rawCommentData.UserId = await userService.GetBaseUserIdAsync(this.User.Id());
            rawCommentData.Username = this.User.Identity.Name;
            rawCommentData.Id = await commentService.AddComment(rawCommentData);

            return Ok(rawCommentData);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCommentAsync(int id)
        {
            var comment = await commentService.GetCommentAsync(id);

            if (comment == null)
            {
                return this.BadRequest();
            }

            if (comment.UserId != await userService.GetBaseUserIdAsync(this.User.Id()) && !this.User.IsAdmin())
            {
                return StatusCode(401,"Only the comment author or site administrator can delete this comment");
            }

            await commentService.DeleteCommentAsync(comment);

            return Ok();
        }
    }
}

