using ASP.NET_MVC_Forum.Web.Models.Chat;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Web.Services.Business.Chat
{
    public interface IChatBusinessService
    {
        public Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(string recipientUsername, string currentIdentityUserId, string currentIdentityUserUsername);
        public Task<T> GenerateChatConversationViewModel<T>(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername);
    }
}
