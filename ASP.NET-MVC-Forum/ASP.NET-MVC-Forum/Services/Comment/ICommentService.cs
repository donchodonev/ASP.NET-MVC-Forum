namespace ASP.NET_MVC_Forum.Services.Comment
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
    using ASP.NET_MVC_Forum.Services.Comment.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;

    public interface ICommentService
    {
        public Task<int> AddComment(RawCommentServiceModel comment);

        public Task<IEnumerable<CommentGetRequestResponseModel>> AllComments(int postId);

        public Task<Comment> GetCommentAsync(int id);

        public Task DeleteCommentAsync(Comment comment);
    }
}
