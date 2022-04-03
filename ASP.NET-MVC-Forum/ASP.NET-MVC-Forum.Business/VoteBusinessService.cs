﻿namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;

    using AutoMapper;

    using System.Linq;
    using System.Threading.Tasks;

    public class VoteBusinessService : IVoteBusinessService
    {
        private readonly IVoteRepository voteRepo;
        private readonly IMapper mapper;

        public VoteBusinessService(IVoteRepository voteRepo, IMapper mapper)
        {
            this.voteRepo = voteRepo;
            this.mapper = mapper;
        }

        public async Task RegisterVote(VoteRequestModel incomingVote, int userId)
        {
            Vote vote = await voteRepo.GetUserVoteAsync(userId, incomingVote.PostId);

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

            await voteRepo.UpdateVoteAsync(vote);
        }

        public async Task<int> GetPostVoteSum(int postId)
        {
            var votes = await voteRepo.GetPostVotesAsync(postId);

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
