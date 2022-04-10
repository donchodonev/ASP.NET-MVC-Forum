namespace ASP.NET_MVC_Forum.Web.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

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

        [HttpPost]
        public async Task<ActionResult<VoteResponseModel>> CastVote(VoteRequestModel vote)
        {
            await voteService.RegisterVoteAsync(vote, User.Id());

            VoteResponseModel response = new VoteResponseModel() { VoteSum = await voteService.GetPostVoteSumAsync(vote.PostId) };

            return Ok(response);
        }
    }
}
