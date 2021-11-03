namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Services.Vote;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [IgnoreAntiforgeryToken(Order = 2000)]////////////////////DONT FORGET/////////////////////
    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService voteService;

        public VotesController(IVoteService voteService)
        {
            this.voteService = voteService;
        }

        [HttpGet]
        public ActionResult<VoteResponseModel> GetVotesSum(int postId)
        {
            return voteService.GetPostVoteSum(postId);
        }

        [HttpPost]
        public ActionResult<VoteRequestModel> CastVote(VoteRequestModel vote)
        {
            var response = voteService.RegisterVote(vote);

            return Ok(response);
        }
    }
}
