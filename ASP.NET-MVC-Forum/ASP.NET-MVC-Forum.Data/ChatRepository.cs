namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models;
    using ASP.NET_MVC_Forum.Infrastructure;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ChatConstants.Errors;

    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IUserDataService userService;
        private readonly IHtmlManipulator htmlManipulator;

        public ChatRepository(ApplicationDbContext db,
            IUserDataService userService,
            IHtmlManipulator htmlManipulator)
        {
            this.db = db;
            this.userService = userService;
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

        public async Task<bool> ChatExistsAsync(string identityUserA, string identityUserB)
        {
            int userA = await userService.GetBaseUserIdAsync(identityUserA);
            int userB = await userService.GetBaseUserIdAsync(identityUserB);

            return db
                .Chats
                .AsNoTracking()
                .Any(x => (x.UserA == userA && x.UserB == userB) || (x.UserA == userB && x.UserB == userA));
        }

        public async Task<long> GetChatIdAsync(string identityUserA, string identityUserB)
        {
            if (!await ChatExistsAsync(identityUserA, identityUserB))
            {
                throw new AppException(CHAT_DOES_NOT_EXIST);
            }

            int userA = await userService.GetBaseUserIdAsync(identityUserA);
            int userB = await userService.GetBaseUserIdAsync(identityUserB);

            return db
                .Chats
                .FirstOrDefaultAsync(x => (x.UserA == userA && x.UserB == userB) || (x.UserA == userB && x.UserB == userA))
                .Id;
        }

        /// <summary>
        /// Creates a new chat and saves it in the database
        /// </summary>
        /// <param name="identityUserA"></param>
        /// <param name="identityUserB"></param>
        /// <returns>Returns the create chat's Id</returns>
        public async Task<long> CreateChatAsync(string identityUserA, string identityUserB)
        {
            int userA = await userService.GetBaseUserIdAsync(identityUserA);
            int userB = await userService.GetBaseUserIdAsync(identityUserB);

            Chat chat = new Chat() { UserA = userA, UserB = userB };

            db.Chats.Add(chat);
            await db.SaveChangesAsync();

            return chat.Id;
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
