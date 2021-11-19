namespace ASP.NET_MVC_Forum.Services.Chat
{
    using System.Threading.Tasks;

    public interface IChatService
    {
        public Task PersistMessageAsync(long chatId, string message);

        public Task<bool> ChatExistsAsync(string identityUserA, string identityUserB);

        public Task<long> GetChatIdAsync(string identityUserA, string identityUserB);

        public Task<long> CreateChatAsync(string identityUserA, string identityUserB);
    }
}
