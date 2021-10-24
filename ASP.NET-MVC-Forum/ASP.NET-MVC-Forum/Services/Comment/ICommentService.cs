using ASP.NET_MVC_Forum.Areas.API.Models;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    public interface ICommentService
    {
        public Task AddComment(RawCommentServiceModel comment);

        public Task<IEnumerable<CommentGetRequestResponseModel>> AllComments(int postId);
    }
}
