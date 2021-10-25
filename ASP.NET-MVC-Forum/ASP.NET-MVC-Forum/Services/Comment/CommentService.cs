﻿using ASP.NET_MVC_Forum.Areas.API.Models;
using ASP.NET_MVC_Forum.Data;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<int> AddComment(RawCommentServiceModel commentData)
        {
            var comment = mapper.Map<ASP.NET_MVC_Forum.Data.Models.Comment>(commentData);

            await db.Comments.AddAsync(comment);

            await db.SaveChangesAsync();

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

        public async Task<ASP.NET_MVC_Forum.Data.Models.Comment> GetCommentAsync(int id)
        {
            return await db.Comments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteCommentAsync(ASP.NET_MVC_Forum.Data.Models.Comment comment)
        {
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
        }
    }
}
