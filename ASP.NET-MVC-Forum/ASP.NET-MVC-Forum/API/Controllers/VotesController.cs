namespace ASP.NET_MVC_Forum.Web.API.Controllers
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

            var voteSum = await voteService.GetPostVoteSumAsync(vote.PostId);

            var response = new VoteResponseModel(voteSum);

            return Ok(response);
        }
    }
}
