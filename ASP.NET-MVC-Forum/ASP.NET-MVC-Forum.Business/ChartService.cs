namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Stats;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ColorConstants;

    public class ChartService : IChartService
    {
        private readonly IPostRepository postRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IMapper mapper;

        public ChartService(
            IPostRepository postRepo,
            ICategoryRepository categoryRepo,
            IMapper mapper)
        {
            this.postRepo = postRepo;
            this.categoryRepo = categoryRepo;
            this.mapper = mapper;
        }

        public async Task<List<MostCommentedPostsResponeModel>> GetMostCommentedPostsChartDataAsync(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            var posts = postRepo
                .All()
                .Where(x => !x.IsDeleted)
                .Include(x => x.Comments);

            var chartData = await GetStatsAs<MostCommentedPostsResponeModel>(count, posts).ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostLikedPostsResponeModel>> GetMostLikedPostsChartDataAsync(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            var posts = postRepo
                .All()
                .Where(x => !x.IsDeleted)
                .Include(x => x.Votes);

            var chartData = await GetStatsAs<MostLikedPostsResponeModel>(count, posts).ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostReportedPostsResponeModel>> GetMostReportedPostsChartDataAsync(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            var posts = postRepo
                .All()
                .Where(x => !x.IsDeleted)
                .Include(x => x.Reports);

            var chartData = await GetStatsAs<MostReportedPostsResponeModel>(count, posts).ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostPostsPerCategoryResponseModel>> GetMostPostsPerCategoryAsync(int count)
        {
            if (count <= 0)
            {
                count = 1;
            }

            var categories = categoryRepo
                .All()
                .Where(x => !x.IsDeleted)
                .Include(x => x.Posts);

            var chartData = await GetStatsAs<MostPostsPerCategoryResponseModel>(count, categories)
                .ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public List<(string chartName, string url)> GenerateChartNamesAndUrls()
        {
            return new List<(string, string)>()
            {
                ( "Most commented posts", "/api/stats/most-commented-posts"),
                ( "Most liked posts", "/api/stats/most-liked-posts"),
                ( "Most reported posts", "/api/stats/most-reported-posts"),
                ( "Most posts by category", "/api/stats/most-posts-by-category")
            };
        }

        public IQueryable<T> GetStatsAs<T>(int count, IQueryable<Category> categories)
        {
            return TakeValidCountOf<Category>(categories, count)
                    .ProjectTo<T>(mapper.ConfigurationProvider);
        }

        private IQueryable<T> TakeValidCountOf<T>(IQueryable<T> items, int requestedCount)
        {
            int itemsTotalCount = items.Count();

            int lowestCountBetweenTotalPostCountAndTheCountOfColors =
                Math.Min(itemsTotalCount,
                Colors.Length);

            int lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount =
                Math.Min(
                    lowestCountBetweenTotalPostCountAndTheCountOfColors,
                    requestedCount);

            if (lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount >= requestedCount)
            {
                return items
                        .Take(requestedCount);
            }

            return items
                    .Take(lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount);
        }

        private IQueryable<T> GetStatsAs<T>(int count, IQueryable<Post> posts)
        {
            return TakeValidCountOf<Post>(posts, count)
                    .ProjectTo<T>(mapper.ConfigurationProvider);
        }

        private void AddColor<T>(List<T> posts) where T : IStatsResponseModel
        {
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].Color = Colors[i];
            }
        }
    }
}
