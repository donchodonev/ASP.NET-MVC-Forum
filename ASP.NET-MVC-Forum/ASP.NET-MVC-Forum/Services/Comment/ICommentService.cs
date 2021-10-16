using ASP.NET_MVC_Forum.Areas.API.Models;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    public interface ICommentService
    {
        public Task AddComment(CommentPostRequestModel comment);
    }
}
