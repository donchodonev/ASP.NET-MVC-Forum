namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class CommentRepository : ICommentRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;

        public CommentRepository(IMapper mapper, ApplicationDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<int> AddCommentAsync(CommentPostRequestModel commentData, string userId)
        {
            var comment = mapper.Map<Comment>(commentData);

            comment.UserId = userId;

            db.Comments.Add(comment);

            await db.SaveChangesAsync();

            return comment.Id;
        }

        public IQueryable<Comment> All()
        {
            return db.Comments;
        }

        public IQueryable GetAllByPostId(int postId)
        {
            var query = db.Comments.Where(x => x.PostId == postId);

            return query;
        }

        public IQueryable<Comment> GetById(int id)
        {
            return db.Comments.Where(x => x.Id == id);
        }

        public Task UpdateAsync(Comment entity)
        {
            db.Update(entity);

            return db.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(int commentId)
        {
            return db
                .Comments
                .AnyAsync(x => x.Id == commentId
                && !x.IsDeleted 
                && x.IsVisible);
        }
    }
}
