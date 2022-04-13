namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ChatConstants.Errors;

    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IHtmlManipulator htmlManipulator;

        public ChatRepository(ApplicationDbContext db,
            IHtmlManipulator htmlManipulator)
        {
            this.db = db;
            this.htmlManipulator = htmlManipulator;
        }

        public async Task<Message> AddMessageAsync(long chatId, string message, string senderUsername)
        {
            var sanitizedMessage = htmlManipulator.Sanitize(message);
            var htmlEscapedMessage = htmlManipulator.Escape(sanitizedMessage);

            Message chatMessage = new Message()
            {
                ChatId = chatId,
                Text = htmlEscapedMessage,
                SenderUsername = senderUsername
            };

            db.Messages.Add(chatMessage);

            await db.SaveChangesAsync();

            return chatMessage;
        }

        public Task<bool> ChatExistsAsync(string identityUserA, string identityUserB)
        {
            return db
                .Chats
                .AsNoTracking()
                .AnyAsync(x =>
                (x.UserA == identityUserA && x.UserB == identityUserB)
                ||
                (x.UserA == identityUserB && x.UserB == identityUserA));
        }

        public async Task<long> GetChatIdAsync(string identityUserA, string identityUserB)
        {
            if (!await ChatExistsAsync(identityUserA, identityUserB))
            {
                throw new AppException(CHAT_DOES_NOT_EXIST);
            }

            return await db
                .Chats
                 .Where(x =>
                     (x.UserA == identityUserA && x.UserB == identityUserB)
                     ||
                     (x.UserA == identityUserB && x.UserB == identityUserA))
                 .Select(x => x.Id)
                 .FirstOrDefaultAsync();
        }

        public async Task CreateChatAsync(string identityUserA, string identityUserB)
        {
            if (!await ChatExistsAsync(identityUserA, identityUserB))
            {
                Chat chat = new Chat() { UserA = identityUserA, UserB = identityUserB };

                db.Chats.Add(chat);

                await db.SaveChangesAsync();
            }
        }

        public IQueryable<Message> GetLastMessagesAsNoTracking(long chatId, int count = 100)
        {
            return db
                .Messages
                .AsNoTracking()
                .Where(x => x.ChatId == chatId)
                .OrderBy(x => x.CreatedOn)
                .Take(count);
        }
    }
}
