namespace ASP.NET_MVC_Forum.Services.Vote
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Data;
    using AutoMapper;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;
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

        public VoteResponseModel RegisterVote(VoteRequestModel incomingVote)
        {
            var vote = mapper.Map<Vote>(incomingVote);

            db.Votes.Add(vote);

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
    }
}
