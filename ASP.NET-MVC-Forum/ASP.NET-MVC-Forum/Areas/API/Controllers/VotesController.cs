namespace ASP.NET_MVC_Forum.Web.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Web.Services.Data.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Web.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IVoteBusinessService voteService;
        private readonly IUserDataService userService;

        public VotesController(IVoteBusinessService voteService, IUserDataService userService)
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
