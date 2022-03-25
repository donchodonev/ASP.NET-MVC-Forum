namespace ASP.NET_MVC_Forum.Web.Services.Business.Vote
{
    using ASP.NET_MVC_Forum.Web.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Web.Services.Data.Vote;
    using AutoMapper;
    using System.Linq;
    using System.Threading.Tasks;

    public class VoteBusinessService : IVoteBusinessService
    {
        private readonly IVoteDataService data;
        private readonly IMapper mapper;

        public VoteBusinessService(IVoteDataService data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task RegisterVote(VoteRequestModel incomingVote, int userId)
        {
            Vote vote = await data.GetUserVoteAsync(userId, incomingVote.PostId);

            if (vote != null)
            {
                vote.VoteType = GetRequestModelVoteType(incomingVote);
            }
            else
            {
                vote = mapper.Map<Vote>(incomingVote);
                vote.UserId = userId;
                //VoteType assigned by mapper logic
            }

            await data.UpdateVoteAsync(vote);
        }

        public async Task<int> GetPostVoteSum(int postId)
        {
            var votes = await data.GetPostVotesAsync(postId);

            return votes.Sum(x => (int)x.VoteType);
        }

        private VoteType GetRequestModelVoteType(VoteRequestModel incomingVote)
        {
            if (incomingVote.IsPositiveVote)
            {
                return VoteType.Like;
            }

            return VoteType.Dislike;
        }
    }
}
