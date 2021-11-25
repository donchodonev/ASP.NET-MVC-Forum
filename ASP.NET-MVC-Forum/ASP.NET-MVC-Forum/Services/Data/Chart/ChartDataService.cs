namespace ASP.NET_MVC_Forum.Services.Data.Chart
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants.ColorConstants;

    public class ChartDataService : IChartDataService
    {
        private readonly IPostDataService postDataService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public ChartDataService(IPostDataService postDataService, ICategoryService categoryService, IMapper mapper)
        {
            this.postDataService = postDataService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public async Task<List<MostCommentedPostsResponeModel>> GetMostCommentedPostsChartDataAsync(int count)
        {
            var posts = await postDataService.All(
               PostQueryFilter.WithoutDeleted,
               PostQueryFilter.AsNoTracking,
               PostQueryFilter.WithComments)
               .OrderByDescending(x => x.Comments.Count)
               .ToListAsync();

            posts = TakeValidCountOf(posts, count);

            var chartData = TransformToPostChartData<MostCommentedPostsResponeModel>(posts);

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostLikedPostsResponeModel>> GetMostLikedPostsChartDataAsync(int count)
        {
            var posts = await postDataService.All(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithVotes)
                .OrderByDescending(x => x.Votes.Sum(x => (int)x.VoteType))
                .ToListAsync();

            posts = TakeValidCountOf(posts, count);

            var chartData = TransformToPostChartData<MostLikedPostsResponeModel>(posts);

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostReportedPostsResponeModel>> GetMostReportedPostsChartDataAsync(int count)
        {
            var posts = await postDataService.All(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithReports)
                .OrderByDescending(x => x.Reports.Count)
                .ToListAsync();

            posts = TakeValidCountOf(posts, count);

            var chartData = TransformToPostChartData<MostReportedPostsResponeModel>(posts);

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostPostsPerCategoryResponseModel>> GetMostPostsPerCategoryAsync(int count)
        {
            var categories = await categoryService
                .All(withPostsIncluded: true)
                .OrderByDescending(x => x.Posts.Count)
                .ToListAsync();

            categories = TakeValidCountOf(categories, count);

            var chartData = TransformToCategoryChartData<MostPostsPerCategoryResponseModel>(categories);

            AddColor(chartData);

            return chartData;
        }

        private List<T> TakeValidCountOf<T>(ICollection<T> posts, int requestedCount)
        {
            int postsTotalCount = posts.Count();

            int lowestCountBetweenTotalPostCountAndTheCountOfColors = 
                Math.Min(postsTotalCount,
                Colors.Length);

            int lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount =
                Math.Min(
                    lowestCountBetweenTotalPostCountAndTheCountOfColors,
                    requestedCount);

            if (lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount >= requestedCount)
            {
                return posts
                    .Take(requestedCount)
                    .ToList();
            }

            return posts
                .Take(lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount)
                .ToList();
        }

        private List<T> TransformToPostChartData<T>(List<Post> posts) where T : class
        {
            List<T> newCollection = new List<T>();

            for (int i = 0; i < posts.Count; i++)
            {
                newCollection.Add(mapper.Map<T>(posts[i]));
            }

            return newCollection;
        }

        private List<T> TransformToCategoryChartData<T>(List<Category> categories) where T : class
        {
            List<T> newCollection = new List<T>();

            for (int i = 0; i < categories.Count; i++)
            {
                newCollection.Add(mapper.Map<T>(categories[i]));
            }

            return newCollection;
        }

        private List<T> AddColor<T>(List<T> posts) where T : IStatsResponseModel
        {
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].Color = Colors[i];
            }

            return posts;
        }
    }
}
