namespace ASP.NET_MVC_Forum.Services.Business.Vote
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using System.Threading.Tasks;

    public interface IVoteBusinessService
    {
        public Task RegisterVote(VoteRequestModel incomingVote, int userId);

        public Task<int> GetPostVoteSum(int postId);
    }
}
