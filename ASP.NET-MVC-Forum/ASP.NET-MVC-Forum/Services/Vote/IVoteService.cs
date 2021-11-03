using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Vote
{
    public interface IVoteService
    {
        public VoteResponseModel RegisterVote(VoteRequestModel incomingVote);

        public VoteResponseModel GetPostVoteSum(int postId);
    }
}
