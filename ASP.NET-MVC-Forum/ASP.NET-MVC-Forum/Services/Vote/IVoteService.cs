
namespace ASP.NET_MVC_Forum.Services.Vote
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Data.Models;
    public interface IVoteService
    {
        public VoteResponseModel RegisterVote(VoteRequestModel incomingVote, int userId);

        public VoteResponseModel GetPostVoteSum(int postId);

        public Vote GetUserVote(int userId, int postId);
    }
}
