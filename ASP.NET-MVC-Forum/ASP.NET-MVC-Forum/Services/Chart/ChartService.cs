namespace ASP.NET_MVC_Forum.Services.Chart
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using ASP.NET_MVC_Forum.Services.Enums;
    using ASP.NET_MVC_Forum.Services.Post;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.ColorConstants;

    public class ChartService : IChartService
    {
        private readonly IPostService postService;
        private readonly IMapper mapper;
        private readonly string[] colors = new string[] { Blue, Navy, Green, Teal, Lime, Aqua, Olive, Purple, Maroon, Yellow };

        public ChartService(IPostService postService, IMapper mapper)
        {
            this.postService = postService;
            this.mapper = mapper;
        }

        public List<MostCommentedPostsResponeModel> GetMostCommentedPostsChartData(int count)
        {
            var posts = postService.AllAsync(
               PostQueryFilter.WithoutDeleted,
               PostQueryFilter.AsNoTracking,
               PostQueryFilter.WithComments)
               .GetAwaiter().GetResult()
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
            var posts = postService.AllAsync(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithVotes)
                .GetAwaiter().GetResult()
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
            var posts = postService.AllAsync(
                PostQueryFilter.WithoutDeleted,
                PostQueryFilter.AsNoTracking,
                PostQueryFilter.WithReports)
                .GetAwaiter().GetResult()
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
    }
}
