namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IChatRepository
    {
        public Task<Message> AddMessageAsync(long chatId, string message, string senderUsername);

        public Task<bool> ExistsAsync(string identityUserA, string identityUserB);

        public Task<bool> ExistsAsync(long chatId);

        public Task<long> GetChatIdAsync(string identityUserA, string identityUserB);

        public Task EnsureChatExistsAsync(string identityUserA, string identityUserB);

        public IQueryable<Message> GetLastMessagesAsNoTracking(long chatId, int count = 100);
    }
}
