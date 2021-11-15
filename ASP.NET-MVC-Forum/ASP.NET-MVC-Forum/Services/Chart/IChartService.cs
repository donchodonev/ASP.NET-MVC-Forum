using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
using System.Collections.Generic;

namespace ASP.NET_MVC_Forum.Services.Chart
{
    public interface IChartService
    {
        public List<MostCommentedPostsResponeModel> GetMostCommentedPostsChartData(int count);
        public List<MostLikedPostsResponeModel> GetMostLikedPostsChartData(int count);
        public List<MostReportedPostsResponeModel> GetMostReportedPostsChartData(int count);
        public List<MostPostsPerCategoryResponseModel> GetMostPostsPerCategory(int count);

    }
}
