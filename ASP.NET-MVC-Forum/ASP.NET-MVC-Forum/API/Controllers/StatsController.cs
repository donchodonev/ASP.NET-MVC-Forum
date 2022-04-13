namespace ASP.NET_MVC_Forum.Web.API.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Stats;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        public async Task<ActionResult<List<MostCommentedPostsResponeModel>>> GetMostCommentedPosts(int resultCount = 7)
        {
            var chartData = await chartService
                .GetMostCommentedPostsChartDataAsync(resultCount);

            return Ok(new { chartData, fileDownLoadName = "Top posts ordered descending by comments count" });
        }

        [Route("most-liked-posts/{count:int?}")]
        public async Task<ActionResult<List<MostCommentedPostsResponeModel>>> GetMostLikedPosts(int resultCount = 7)
        {
            var chartData = await chartService
                .GetMostLikedPostsChartDataAsync(resultCount);

            return Ok(new { chartData, fileDownLoadName = "Top posts ordered descending by vote sum" });
        }

        [Route("most-reported-posts/{count:int?}")]
        public async Task<ActionResult<List<MostCommentedPostsResponeModel>>> GetReportedPosts(int resultCount = 7)
        {
            var chartData = await chartService
                .GetMostReportedPostsChartDataAsync(resultCount);

            return Ok(new { chartData, fileDownLoadName = "Top posts ordered descending by reports count" });
        }

        [Route("most-posts-by-category/{count:int?}")]
        public async Task<ActionResult<List<MostPostsPerCategoryResponseModel>>> GetMostPostsPerCategory(int resultCount = 7)
        {
            var chartData = await chartService.GetMostPostsPerCategoryAsync(resultCount);

            return Ok(new { chartData, fileDownLoadName = "Top categories ordered descending by posts count" });
        }
    }
}
