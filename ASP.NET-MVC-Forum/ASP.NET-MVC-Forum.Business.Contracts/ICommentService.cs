namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        public Task<List<CommentGetRequestResponseModel>> GenerateCommentGetRequestResponseModelAsync(int postId);

        public Task<CommentPostResponseModel> GenerateCommentResponseModelAsync(
            CommentPostRequestModel commentData,
            ClaimsPrincipal user, 
            int commentId);

        public Task DeleteAsync(int commentId, ClaimsPrincipal user);
    }
}
