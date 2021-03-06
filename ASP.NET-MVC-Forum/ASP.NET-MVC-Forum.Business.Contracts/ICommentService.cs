namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommentService
    {
        public Task<List<CommentGetRequestResponseModel>> GenerateCommentGetResponseModelAsync(int postId);

        public Task<CommentPostResponseModel> GenerateCommentPostResponseModelAsync(
               CommentPostRequestModel commentData,
               string userId,
               string userUsername,
               int commentId);

        public Task DeleteAsync(int commentId,
            string userId,
            bool isInAdminOrModeratorRole);
    }
}
