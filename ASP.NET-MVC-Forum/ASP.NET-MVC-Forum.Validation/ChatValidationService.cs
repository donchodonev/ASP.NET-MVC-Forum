namespace ASP.NET_MVC_Forum.Validation
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ChatConstants.Errors;

    public class ChatValidationService : IChatValidationService
    {
        private readonly IChatRepository chatRepo;

        public ChatValidationService(IChatRepository chatRepo)
        {
            this.chatRepo = chatRepo;
        }

        public async Task ValidateChatExistsAsync(long chatId)
        {
            if(!await chatRepo.ExistsAsync(chatId))
            {
                throw new EntityDoesNotExistException(CHAT_DOES_NOT_EXIST);
            }
        }
    }
}
