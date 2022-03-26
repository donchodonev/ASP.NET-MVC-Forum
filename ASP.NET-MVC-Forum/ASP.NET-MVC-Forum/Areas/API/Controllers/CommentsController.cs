namespace ASP.NET_MVC_Forum.Web.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentBusinessService commentService;

        public CommentsController(ICommentBusinessService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IEnumerable<CommentGetRequestResponseModel>> GetComments(int postId)
        {
            return await commentService.GenerateCommentGetRequestResponseModel(postId);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentPostRequestModel>> AddComment(CommentPostRequestModel commentData)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            var rawCommentData = await commentService.GenerateRawCommentServiceModel(commentData, this.User);

            return Ok(rawCommentData);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCommentAsync(int id)
        {
            if (!await commentService.CommentExistsAsync(id))
            {
                return this.BadRequest();
            }

            if (!await commentService.IsUserPrivileged(id, this.User))
            {
                return StatusCode(401, "Only the comment author or site administrator can delete this comment");
            }

            await commentService.DeleteAsync(id);

            return Ok();
        }
    }
}

