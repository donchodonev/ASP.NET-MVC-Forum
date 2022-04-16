namespace ASP.NET_MVC_Forum.Web.API.Controllers
{
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Contracts;

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService commentService;
        private readonly ICommentRepository commentRepo;

        public CommentsController(ICommentService commentService,ICommentRepository commentRepo)
        {
            this.commentService = commentService;
            this.commentRepo = commentRepo;
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
            var commentId = await commentRepo.AddCommentAsync(commentData, this.User);

            var rawCommentData = await commentService.GenerateCommentResponseModel(commentData, this.User, commentId);

            return Ok(rawCommentData);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCommentAsync(int id)
        {
            await commentService.DeleteAsync(id, this.User);

            return Ok();
        }
    }
}

