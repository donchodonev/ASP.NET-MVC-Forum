namespace ASP.NET_MVC_Forum.Services.Chart
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.ColorConstants;

    public class ChartService : IChartService
    {
        private readonly IPostDataService postService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly string[] colors = new string[] { Blue, Navy, Green, Teal, Lime, Aqua, Olive, Purple, Maroon, Yellow };

        public ChartService(IPostDataService postService, ICategoryService categoryService, IMapper mapper)
        {
            this.postService = postService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public List<MostCommentedPostsResponeModel> GetMostCommentedPostsChartData(int count)
        {
            var posts = postService.All(
               PostQueryFilter.WithoutDeleted,
               PostQueryFilter.AsNoTracking,
               PostQueryFilter.WithComments)
               .OrderByDescending(x => x.Comments.Count)
               .ToList();

            int postsTotalCount = posts.Count();

            if (postsTotalCount <= count && postsTotalCount <= colors.Length)
            {
                posts = posts
                    .Take(postsTotalCount)
                    .ToList();
            }
            else
            {
                posts = posts
                    .Take(colors.Length)
                    .ToList();
            }

            var chartData = new List<MostCommentedPostsResponeModel>();

            for (int i = 0; i < posts.Count; i++)
            {
                var postData = mapper.Map<MostCommentedPostsResponeModel>(posts[i]);
                postData.Color = colors[i];
                chartData.Add(postData);
            }

            return chartData;
        }

        public List<MostLikedPostsResponeModel> GetMostLikedPostsChartData(int count)
        {
            var posts = postService.All(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithVotes)
                .OrderByDescending(x => x.Votes.Sum(x => (int)x.VoteType))
                .ToList();

            int postsTotalCount = posts.Count();

            if (postsTotalCount <= count && postsTotalCount <= colors.Length)
            {
                posts = posts
                    .Take(postsTotalCount)
                    .ToList();
            }
            else
            {
                posts = posts
                    .Take(colors.Length)
                    .ToList();
            }

            var chartData = new List<MostLikedPostsResponeModel>();

            for (int i = 0; i < posts.Count; i++)
            {
                var postData = mapper.Map<MostLikedPostsResponeModel>(posts[i]);
                postData.Color = colors[i];
                chartData.Add(postData);
            }

            return chartData;
        }

        public List<MostReportedPostsResponeModel> GetMostReportedPostsChartData(int count)
        {
            var posts = postService.All(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithReports)
                .OrderByDescending(x => x.Reports.Count)
                .ToList();

            int postsTotalCount = posts.Count();

            if (postsTotalCount <= count && postsTotalCount <= colors.Length)
            {
                posts = posts
                    .Take(postsTotalCount)
                    .ToList();
            }
            else
            {
                posts = posts
                    .Take(colors.Length)
                    .ToList();
            }

            var chartData = new List<MostReportedPostsResponeModel>();

            for (int i = 0; i < posts.Count; i++)
            {
                var postData = mapper.Map<MostReportedPostsResponeModel>(posts[i]);
                postData.Color = colors[i];
                chartData.Add(postData);
            }

            return chartData;
        }

        public List<MostPostsPerCategoryResponseModel> GetMostPostsPerCategory(int count)
        {
            var categories = categoryService
                .All(withPostsIncluded: true)
                .OrderByDescending(x => x.Posts.Count)
                .ToList();

            int categoriesTotalCount = categories.Count();

            if (categoriesTotalCount <= count && categoriesTotalCount <= colors.Length)
            {
                categories = categories
                    .Take(categoriesTotalCount)
                    .ToList();
            }
            else
            {
                categories = categories
                    .Take(colors.Length)
                    .ToList();
            }

            var chartData = new List<MostPostsPerCategoryResponseModel>();

            for (int i = 0; i < categories.Count; i++)
            {
                var postData = mapper.Map<MostPostsPerCategoryResponseModel>(categories[i]);
                postData.Color = colors[i];
                chartData.Add(postData);
            }

            return chartData;
        }
    }
}
