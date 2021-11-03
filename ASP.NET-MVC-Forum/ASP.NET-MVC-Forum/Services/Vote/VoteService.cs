namespace ASP.NET_MVC_Forum.Services.Vote
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Data;
    using AutoMapper;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;

    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public VoteService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public int RegisterVote(VoteRequestModel incomingVote)
        {
            var vote = mapper.Map<Vote>(incomingVote);

            return vote.Id;
        }




    }
}
