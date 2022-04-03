namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IVoteRepository
    {
        public Task UpdateVoteAsync(Vote vote);

        public Task<List<Vote>> GetPostVotesAsync(int postId);

        public Task<Vote> GetUserVoteAsync(string userId, int postId);
    }
}
