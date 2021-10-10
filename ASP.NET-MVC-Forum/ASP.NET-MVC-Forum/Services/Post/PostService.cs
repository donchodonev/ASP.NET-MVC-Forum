namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext db;

        public PostService(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Returns all posts with or without their category included
        /// </summary>
        /// <param name="withCategory">True to include post's Category property, false for a null Category property</param>
        /// <returns></returns>
        public async Task<IQueryable<Post>> AllAsync(bool withCategoryIncluded = false)
        {
            return await Task.Run(() => 
            {
                var query = db.Posts.AsQueryable();

                if (withCategoryIncluded)
                {
                    query = query.Include(x => x.Category);
                }

                return query;
            });
        }
    }
}
