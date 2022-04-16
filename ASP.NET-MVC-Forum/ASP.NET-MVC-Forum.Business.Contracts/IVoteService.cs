namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using System.Threading.Tasks;

    public interface IVoteService
    {
        public Task RegisterVoteAsync(VoteRequestModel incomingVote, string userId);

        public Task<int> GetPostVoteSumAsync(int postId);

        public Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId);
    }
}
