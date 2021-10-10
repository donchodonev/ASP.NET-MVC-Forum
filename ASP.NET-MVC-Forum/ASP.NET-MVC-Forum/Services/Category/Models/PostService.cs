namespace ASP.NET_MVC_Forum.Services.Category.Models
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Category.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext db;

        public PostService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IQueryable<Post>> AllAsync()
        {
            return await Task.Run(() => db.Posts);
        }
    }
}
