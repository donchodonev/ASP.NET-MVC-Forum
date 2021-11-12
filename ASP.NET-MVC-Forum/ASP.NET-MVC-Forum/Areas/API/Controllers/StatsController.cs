
namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using ASP.NET_MVC_Forum.Services.Chart;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IChartService chartService;

        public StatsController(IChartService chartService)
        {
            this.chartService = chartService;
        }

        [Route("most-commented-posts/{count:int?}")]
        public ActionResult<List<MostCommentedPostsResponeModel>> GetMostCommentedPosts(int resultCount = 7) // count can be changed in the future
        {
            var chartData = chartService.GetMostCommentedPostsChartData(resultCount);

            return Ok(new {chartData, fileDownLoadName = "Top posts ordered descending by comments count" });
        }

        [Route("most-liked-posts/{count:int?}")]
        public ActionResult<List<MostCommentedPostsResponeModel>> GetMostLikedPosts(int resultCount = 7) // count can be changed in the future
        {
            var chartData = chartService.GetMostLikedPostsChartData(resultCount);

            return Ok(new { chartData, fileDownLoadName = "Top posts ordered descending by vote sum" });
        }
    }
}
