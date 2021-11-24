namespace ASP.NET_MVC_Forum.Services.Data.Chart
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChartDataService
    {
        public Task<List<MostCommentedPostsResponeModel>> GetMostCommentedPostsChartDataAsync(int count);
        public Task<List<MostLikedPostsResponeModel>> GetMostLikedPostsChartDataAsync(int count);
        public Task<List<MostReportedPostsResponeModel>> GetMostReportedPostsChartDataAsync(int count);
        public Task<List<MostPostsPerCategoryResponseModel>> GetMostPostsPerCategoryAsync(int count);
    }
}
