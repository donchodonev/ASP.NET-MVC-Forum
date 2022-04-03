namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Data.QueryBuilders;
    using ASP.NET_MVC_Forum.Domain.Models.Stats;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ColorConstants;

    public class ChartService : IChartService
    {
        private readonly IChartRepository chartRepo;
        private readonly IPostRepository postRepo;
        private readonly ICategoryRepository categoryRepo;

        public ChartService(IChartRepository chartRepo, IPostRepository postRepo,
            ICategoryRepository categoryRepo)
        {
            this.chartRepo = chartRepo;
            this.postRepo = postRepo;
            this.categoryRepo = categoryRepo;
        }

        public async Task<List<MostCommentedPostsResponeModel>> GetMostCommentedPostsChartDataAsync(int count)
        {
            var posts = new PostQueryBuilder(postRepo.All())
                .WithoutDeleted()
                .IncludeComments()
                .BuildQuery();

            var chartData = await chartRepo
                .GetStatsAs<MostCommentedPostsResponeModel>(count, posts)
                .ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostLikedPostsResponeModel>> GetMostLikedPostsChartDataAsync(int count)
        {
            var posts = new PostQueryBuilder(postRepo.All())
                .WithoutDeleted()
                .IncludeVotes()
                .BuildQuery();

            var chartData = await chartRepo
                .GetStatsAs<MostLikedPostsResponeModel>(count, posts)
                .ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostReportedPostsResponeModel>> GetMostReportedPostsChartDataAsync(int count)
        {
            var posts = new PostQueryBuilder(postRepo.All())
                .WithoutDeleted()
                .IncludeReports()
                .BuildQuery();

            var chartData = await chartRepo
                .GetStatsAs<MostReportedPostsResponeModel>(count, posts)
                .ToListAsync();

            AddColor(chartData);

            return chartData;
        }

        public async Task<List<MostPostsPerCategoryResponseModel>> GetMostPostsPerCategoryAsync(int count)
        {
            var categories = new CategoryQueryBuilder(categoryRepo.All())
                .WithoutDeleted()
                .IncludePosts()
                .BuildQuery();


            var chartData = await chartRepo
                .GetStatsAs<MostPostsPerCategoryResponseModel>(count, categories)
                .ToListAsync();

            AddColor(chartData);

            return chartData;
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
