namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<AddPostFormModel> GenerateAddPostFormModelAsync();

        public Task<AddPostResponseModel> CreateNewAsync(AddPostFormModel postFormModel, string userId);

        public Task<Post> EditAsync(
            EditPostFormModel viewModelData,
            string userId,
            bool isAdminOrModerator);

        public Task DeleteAsync(
            int postId,
            string userId,
            bool isUserAdminOrModerator);

        public IQueryable<T> GetFilteredAs<T>(
            int sortType,
            int sortOrder,
            string searchTerm,
            string category);

        public Task<T> GetPostByIdAs<T>(int postId);

        public Task<EditPostFormModel> GenerateEditPostFormModelAsync(
            int postId,
            string userId,
            bool isAdminOrModerator);
    }
}