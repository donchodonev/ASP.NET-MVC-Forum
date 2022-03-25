namespace ASP.NET_MVC_Forum.Web.Services.Data.Chat
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Web.Services.Data.User;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Web.Services.Business.HtmlManipulator;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    public class ChatDataService : IChatDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IUserDataService userService;
        private readonly IHtmlManipulator htmlManipulator;

        public ChatDataService(ApplicationDbContext db, IUserDataService userService,IHtmlManipulator htmlManipulator)
        {
            this.db = db;
            this.userService = userService;
            this.htmlManipulator = htmlManipulator;
        }
        public async Task<Message> PersistMessageAsync(long chatId, string message, string senderUsername)
        {
            var sanitizedMessage = htmlManipulator.Sanitize(message);
            var htmlEscapedMessage = htmlManipulator.Escape(sanitizedMessage);

            Message chatMessage = new Message()
            {
                ChatId = chatId,
                Text = htmlEscapedMessage,
                SenderUsername = senderUsername
            };

            await db.Messages.AddAsync(chatMessage);

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
                throw new InvalidOperationException("There is no existing chat between the two users");
            }

            int userA = await userService.GetBaseUserIdAsync(identityUserA);
            int userB = await userService.GetBaseUserIdAsync(identityUserB);

            return db
                .Chats
                .AsNoTracking()
                .First(x => (x.UserA == userA && x.UserB == userB) || (x.UserA == userB && x.UserB == userA))
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

            await db.Chats.AddAsync(chat);
            await db.SaveChangesAsync();

            return chat.Id;
        }

        public IQueryable<Message> GetLastMessages(long chatId, int count = 100)
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
