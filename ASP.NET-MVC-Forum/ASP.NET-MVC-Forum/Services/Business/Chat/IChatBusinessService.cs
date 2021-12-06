using ASP.NET_MVC_Forum.Models.Chat;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.Chat
{
    public interface IChatBusinessService
    {
        public Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(string recipientUsername, string currentIdentityUserId, string currentIdentityUserUsername);
    }
}
