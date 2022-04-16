namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using System.Linq;
    using System.Threading.Tasks;

    public class VoteService : IVoteService
    {
        private readonly IVoteRepository voteRepo;
        private readonly IMapper mapper;
        private readonly IUserValidationService userValidationService;
        private readonly IPostValidationService postValidation;

        public VoteService(
            IVoteRepository voteRepo, 
            IMapper mapper,
            IUserValidationService userValidationService,
            IPostValidationService postValidation)
        {
            this.voteRepo = voteRepo;
            this.mapper = mapper;
            this.userValidationService = userValidationService;
            this.postValidation = postValidation;
        }

        public async Task RegisterVoteAsync(VoteRequestModel incomingVote, string userId)
        {
            await userValidationService.ValidateUserExistsByIdAsync(userId);

            Vote vote = await voteRepo.GetUserVoteAsync(userId, incomingVote.PostId);

            if (vote != null)
            {
                vote.VoteType = GetRequestModelVoteType(incomingVote);
            }
            else
            {
                vote = mapper.Map<Vote>(incomingVote);

                vote.UserId = userId;
            }

            await voteRepo.UpdateVoteAsync(vote);
        }

        public async Task<int> GetPostVoteSumAsync(int postId)
        {
            await postValidation.ValidatePostExistsAsync(postId);

            var votes = await voteRepo.GetPostVotesAsync(postId);

            return votes.Sum(x => (int)x.VoteType);
        }

        public async Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId)
        {
            await userValidationService.ValidateUserExistsByIdAsync(identityUserId);

            var vote = await voteRepo.GetUserVoteAsync(identityUserId, viewModel.PostId);

            viewModel.UserLastVoteChoice = vote switch
            {
                null => 0,
                _ => (int)vote.VoteType
            };
        }

        private VoteType GetRequestModelVoteType(VoteRequestModel incomingVote)
        {
            return incomingVote.IsPositiveVote ? VoteType.Like : VoteType.Dislike;
        }
    }
}
