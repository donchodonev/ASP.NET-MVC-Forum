﻿namespace ASP.NET_MVC_Forum.Services.Data.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostDataService
    {
        public  IQueryable<Post> All(params PostQueryFilter[] filters);

        public Task<int> AddPostAsync(Post post);

        public Task<Post> GetByIdAsync(int postId, params PostQueryFilter[] filters);

        public IQueryable<Post> GetByCategory(int categoryId, params PostQueryFilter[] filters);

        public Task<bool> PostExistsAsync(string postTitle);

        public Task<bool> PostExistsAsync(int postId);

        public IQueryable<Post> GetByUserId(int userId, params PostQueryFilter[] filters);

        public IQueryable<Post> GetByIdAsQueryable(int postId, params PostQueryFilter[] filters);

        public Task UpdatePostAsync(Post post);

        public Task<bool?> IsPostDeleted(int postId, string postTitle);

        public Task Delete(Post post);
    }
}
