namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using System.Threading.Tasks;

    public interface IVoteService
    {
        public Task RegisterVote(VoteRequestModel incomingVote, string userId);

        public Task<int> GetPostVoteSum(int postId);
    }
}
