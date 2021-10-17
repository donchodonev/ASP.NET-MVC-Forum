using ASP.NET_MVC_Forum.Areas.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    public interface ICommentService
    {
        public Task AddComment(CommentPostRequestModel comment);

        public Task<IEnumerable<CommentGetRequestResponseModel>> AllComments(int postId);
    }
}
