using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        public StatsController()
        {

        }

        [Route("most-commented-posts/{count:int?}")]
        public ActionResult GetMostCommentedPosts(int resultCount = 5)
        {
            return this.LocalRedirect("");
        }
    }
}
