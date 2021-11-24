namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Services.Business.Vote;
    using ASP.NET_MVC_Forum.Services.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IVoteBusinessService voteService;
        private readonly IUserService userService;

        public VotesController(IVoteBusinessService voteService, IUserService userService)
        {
            this.voteService = voteService;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<VoteResponseModel>> CastVote(VoteRequestModel vote)
        {
            int userId = await userService
                .GetBaseUserIdAsync(this.User.Id());

            await voteService.RegisterVote(vote, userId);

            VoteResponseModel response = new VoteResponseModel() { VoteSum = await voteService.GetPostVoteSum(vote.PostId) };

            return Ok(response);
        }
    }
}
