namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using System.Threading.Tasks;

    public interface IChatValidationService
    {
        public Task ValidateChatExistsAsync(long chatId);
    }
}
