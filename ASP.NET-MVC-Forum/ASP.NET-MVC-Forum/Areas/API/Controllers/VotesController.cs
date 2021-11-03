namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [IgnoreAntiforgeryToken(Order = 2000)]////////////////////DONT FORGET/////////////////////
    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        public VotesController()
        {

        }




        [HttpPost]
        public ActionResult<VoteRequestModel> CastVote(VoteRequestModel vote)
        {
            return Ok(vote);
        }
    }
}
