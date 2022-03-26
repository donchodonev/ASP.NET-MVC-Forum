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

    public class CategoryDataService : ICategoryDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public CategoryDataService(ApplicationDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
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

        public List<string> GetCategoryNames()
        {
            return db
                .Categories
                .AsNoTracking()
                .Select(x => x.Name)
                .ToList();
        }

        public CategoryIdAndNameViewModel[] GetCategoryIdAndNameCombinations()
        {
            var categories = All();

            var selectOptions = categories
                .ProjectTo<CategoryIdAndNameViewModel>(mapper.ConfigurationProvider)
                .ToArray();

            return selectOptions;
        }
    }
}
