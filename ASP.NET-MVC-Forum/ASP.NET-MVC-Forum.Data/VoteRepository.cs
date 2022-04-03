namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext db;

        public VoteRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public Task UpdateVoteAsync(Vote vote)
        {
            db.Update(vote);

            return db.SaveChangesAsync();
        }

        public Task<List<Vote>> GetPostVotesAsync(int postId)
        {
            return db
                 .Votes
                 .Where(x => x.PostId == postId)
                 .ToListAsync();
        }

        public Task<Vote> GetUserVoteAsync(string userId, int postId)
        {
            return db
                .Votes
                .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }
    }
}
