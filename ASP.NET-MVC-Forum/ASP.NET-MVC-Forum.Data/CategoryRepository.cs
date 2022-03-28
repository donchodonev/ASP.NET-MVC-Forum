namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public CategoryRepository(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public IQueryable<T> AllAs<T>(bool includePosts = false)
        {
            var query = db
                        .Categories
                        .AsQueryable();

            if (includePosts)
            {
                query = query.Include(x => x.Posts);
            }

            return query.ProjectTo<T>(mapper.ConfigurationProvider);
        }

        public IQueryable<Category> All(bool withPostsIncluded = false)
        {
            var query = db
                .Categories
                .AsQueryable();

            if (withPostsIncluded)
            {
                query = query.Include(x => x.Posts);
            }

            return query;
        }

        public Task<List<string>> GetCategoryNamesAsync()
        {
            return  db
                    .Categories
                    .Select(x => x.Name)
                    .ToListAsync();
        }

        public Task<CategoryIdAndNameViewModel[]> GetCategoryIdAndNameCombinationsAsync()
        {
            return AllAs<CategoryIdAndNameViewModel>().ToArrayAsync();
        }
    }
}
