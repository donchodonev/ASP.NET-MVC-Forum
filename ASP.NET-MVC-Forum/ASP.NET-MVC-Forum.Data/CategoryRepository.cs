namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        private const string CATEGORY_ID_AND_NAME_CACHE_KEY = "category_id_and_names";
        private const string CATEGORY_NAMES_CACHEKEY = "category_names";

        public CategoryRepository(ApplicationDbContext db, IMapper mapper, IMemoryCache memoryCache)
        {
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
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

        public async Task<List<string>> GetCategoryNamesAsync()
        {
            if (!memoryCache.TryGetValue<List<string>>(
                CATEGORY_NAMES_CACHEKEY,
                out List<string> names))
            {
                names = await db
                            .Categories
                            .Select(x => x.Name)
                            .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(300),
                };

                memoryCache.Set(CATEGORY_NAMES_CACHEKEY, names, cacheEntryOptions);
            }

            return names;
        }

        public async Task<CategoryIdAndNameViewModel[]> GetCategoryIdAndNameCombinationsAsync()
        {
            if (!memoryCache.TryGetValue<CategoryIdAndNameViewModel[]>(
                CATEGORY_ID_AND_NAME_CACHE_KEY,
                out CategoryIdAndNameViewModel[] idAndNamesArray))
            {
                idAndNamesArray = await AllAs<CategoryIdAndNameViewModel>().ToArrayAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(300),
                };

                memoryCache.Set(CATEGORY_ID_AND_NAME_CACHE_KEY, idAndNamesArray, cacheEntryOptions);
            }

            return idAndNamesArray;
        }
    }
}
