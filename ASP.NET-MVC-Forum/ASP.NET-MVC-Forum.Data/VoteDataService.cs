namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class VoteDataService : IVoteDataService
    {
        private readonly ApplicationDbContext db;

        public VoteDataService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task UpdateVoteAsync(Vote vote)
        {
            db.Update(vote);
            await db.SaveChangesAsync();
        }

        public async Task<List<Vote>> GetPostVotesAsync(int postId)
        {
            return await db
                 .Votes
                 .Where(x => x.PostId == postId)
                 .ToListAsync();
        }

        public async Task<Vote> GetUserVoteAsync(int userId, int postId)
        {
            return await db
                .Votes
                .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }
    }
}
