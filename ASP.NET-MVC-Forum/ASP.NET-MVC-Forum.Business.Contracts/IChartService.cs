namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Stats;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChartService
    {
        public Task<List<MostCommentedPostsResponeModel>> GetMostCommentedPostsChartDataAsync(int count);

        public Task<List<MostLikedPostsResponeModel>> GetMostLikedPostsChartDataAsync(int count);

        public Task<List<MostReportedPostsResponeModel>> GetMostReportedPostsChartDataAsync(int count);

        public Task<List<MostPostsPerCategoryResponseModel>> GetMostPostsPerCategoryAsync(int count);

        public List<(string chartName, string url)> GenerateChartNamesAndUrls();
    }
}
