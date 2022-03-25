namespace ASP.NET_MVC_Forum.Web.Services.Data.Vote
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Domain.Entities;

    public interface IVoteDataService
    {
        public Task<Vote> GetUserVoteAsync(int userId, int postId);

        public Task UpdateVoteAsync(Vote vote);

        public Task<List<Vote>> GetPostVotesAsync(int postId);
    }
}
