using ASP.NET_MVC_Forum.Data;
using ASP.NET_MVC_Forum.Services.User;
namespace ASP.NET_MVC_Forum.Services.Chat
{
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext db;
        private readonly IUserService userService;

        public ChatService(ApplicationDbContext db, IUserService userService)
        {
            this.db = db;
            this.userService = userService;
        }
        public async Task PersistMessageAsync(long chatId, string message)
        {
            Message chatMessage = new Message()
            {
                ChatId = chatId,
                Text = message
            };

            await db.Messages.AddAsync(chatMessage);

            await db.SaveChangesAsync();
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
                .OrderByDescending(x => x.CreatedOn)
                .Take(count);
        }
    }
}
