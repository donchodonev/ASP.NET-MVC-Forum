namespace ASP.NET_MVC_Forum.Services.Chat
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IChatService
    {
        public Task<Message> PersistMessageAsync(long chatId, string message, string senderUsername);

        public Task<bool> ChatExistsAsync(string identityUserA, string identityUserB);

        public Task<long> GetChatIdAsync(string identityUserA, string identityUserB);

        public Task<long> CreateChatAsync(string identityUserA, string identityUserB);

        public IQueryable<Message> GetLastMessages(long chatId, int count = 100);
    }
}
