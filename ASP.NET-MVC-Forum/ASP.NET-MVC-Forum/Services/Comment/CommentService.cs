using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
using ASP.NET_MVC_Forum.Data;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.CommentReport;

    public class CommentService : ICommentService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;
        private readonly ICommentReportService commentReportService;

        public CommentService(IMapper mapper, ApplicationDbContext db, ICommentReportService commentReportService)
        {
            this.mapper = mapper;
            this.db = db;
            this.commentReportService = commentReportService;
        }
        public async Task<int> AddComment(RawCommentServiceModel commentData)
        {
            var comment = mapper.Map<Comment>(commentData);

            await db.Comments.AddAsync(comment);

            await db.SaveChangesAsync();

            commentReportService.AutoGenerateCommentReport(comment.Content,comment.Id);

            return comment.Id;
        }

        public async Task<IEnumerable<CommentGetRequestResponseModel>> AllComments(int postId)
        {
            return await Task.Run(() =>
            {
                var comments = mapper
                .ProjectTo<CommentGetRequestResponseModel>(db
                    .Comments
                    .Include(x => x.User)
                    .ThenInclude(x => x.IdentityUser)
                    .Where(x => x.PostId == postId && !x.IsDeleted))
                .ToArray()
                .Reverse(); // to show most recent comments first

                return comments;
            });
        }

        public async Task<Comment> GetCommentAsync(int id)
        {
            return await db.Comments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
        }
    }
}
