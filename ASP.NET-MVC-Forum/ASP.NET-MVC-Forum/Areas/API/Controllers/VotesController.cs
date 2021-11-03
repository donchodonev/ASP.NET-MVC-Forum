namespace ASP.NET_MVC_Forum.Areas.API.Controllers
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Services.User;
    using ASP.NET_MVC_Forum.Services.Vote;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService voteService;
        private readonly IUserService userService;

        public VotesController(IVoteService voteService,IUserService userService)
        {
            this.voteService = voteService;
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<VoteResponseModel> GetVotesSum(int postId)
        {
            return voteService.GetPostVoteSum(postId);
        }

        [HttpPost]
        public ActionResult<VoteResponseModel> CastVote(VoteRequestModel vote)
        {
            int userId = userService
                .GetBaseUserIdAsync(this.User.Id())
                .GetAwaiter()
                .GetResult();

            var response = voteService.RegisterVote(vote, userId);

            return Ok(response);
        }
    }
}
