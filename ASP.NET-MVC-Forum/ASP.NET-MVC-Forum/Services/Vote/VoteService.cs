namespace ASP.NET_MVC_Forum.Services.Vote
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using AutoMapper;
    using System.Linq;

    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public VoteService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public VoteResponseModel RegisterVote(VoteRequestModel incomingVote,int userId)
        {
            Vote vote = GetUserVote(userId, incomingVote.PostId);

            if (vote != null)
            {
                if (incomingVote.IsPositiveVote)
                {
                    vote.VoteType = VoteType.Like;
                }
                else
                {
                    vote.VoteType = VoteType.Dislike;
                }

                db.Update(vote);
            }
            else
            {
                vote = mapper.Map<Vote>(incomingVote);
                vote.UserId = userId;
                db.Votes.Add(vote);
            }

            db.SaveChangesAsync().Wait();

            return GetPostVoteSum(incomingVote.PostId);
        }

        public VoteResponseModel GetPostVoteSum(int postId)
        {
            var response = new VoteResponseModel();

            response.VoteSum = db
                .Votes
                .Where(x => x.PostId == postId)
                .Sum(x => (int)x.VoteType);

            return response;
        }

        public Vote GetUserVote(int userId, int postId)
        {
            return db
                .Votes
                .FirstOrDefault(x => x.PostId == postId && x.UserId == userId);
        }
    }
}
