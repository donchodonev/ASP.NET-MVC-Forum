using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.Comment
{
    public interface ICommentBusinessService
    {
        public Task<IEnumerable<CommentGetRequestResponseModel>> GenerateCommentGetRequestResponseModel(int postId);

        public Task<RawCommentServiceModel> GenerateRawCommentServiceModel(CommentPostRequestModel commentData, ClaimsPrincipal user);

        public Task<bool> CommentExistsAsync(int commentId);

        public Task<bool> IsUserPrivileged(int commentId, ClaimsPrincipal user);

        public Task DeleteAsync(int commentId);
    }
}
