using ASP.NET_MVC_Forum.Areas.API.Models;
using ASP.NET_MVC_Forum.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Comment
{
    public class CommentService : ICommentService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;

        public CommentService(IMapper mapper, ApplicationDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }
        public async Task AddComment(CommentPostRequestModel commentData)
        {
            var comment = mapper.Map<ASP.NET_MVC_Forum.Data.Models.Comment>(commentData);

            await db.Comments.AddAsync(comment);

            await db.SaveChangesAsync();
        }
    }
}
