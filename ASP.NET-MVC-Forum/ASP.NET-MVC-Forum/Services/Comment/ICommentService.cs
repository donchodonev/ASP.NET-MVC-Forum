using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    public interface ICommentService
    {
        public Task<int> AddComment(RawCommentServiceModel comment);

        public Task<IEnumerable<CommentGetRequestResponseModel>> AllComments(int postId);

        public Task<ASP.NET_MVC_Forum.Data.Models.Comment> GetCommentAsync(int id);

        public Task DeleteCommentAsync(ASP.NET_MVC_Forum.Data.Models.Comment comment);
    }
}
