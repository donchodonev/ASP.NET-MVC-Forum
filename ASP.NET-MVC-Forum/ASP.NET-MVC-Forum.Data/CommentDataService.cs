﻿namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    public class CommentDataService : ICommentDataService
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;

        public CommentDataService(IMapper mapper, ApplicationDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<int> AddComment(RawCommentServiceModel commentData)
        {
            var comment = mapper.Map<Comment>(commentData);

            await db.Comments.AddAsync(comment);

            await db.SaveChangesAsync();

            return comment.Id;
        }

        public IQueryable<Comment> All()
        {
            return db
                .Comments
                .AsQueryable();
        }

        public IQueryable GetAllByPostId(int postId, bool withDeleted = false, bool withIdentityUser = false, bool withBaseUser = false)
        {
            var query = db.Comments.Where(x => x.PostId == postId);

            if (!withDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            if (withBaseUser)
            {
                query = query.Include(x => x.User);
            }

            if (withIdentityUser)
            {
                query = query.Include(x => x.User).ThenInclude(x => x.IdentityUser);
            }

            return query;
        }

        public IQueryable<Comment> GetById(int id)
        {
            return db.Comments.Where(x => x.Id == id);
        }

        public async Task UpdateAsync<T>(T entity)
        {
            db.Update(entity);
            await db.SaveChangesAsync();
        }
    }
}