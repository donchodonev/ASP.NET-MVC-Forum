namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<AddPostFormModel> GeneratedAddPostFormModelAsync();

        public Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel post, string userId);

        public Task<Post> Edit(EditPostFormModel viewModelData, ClaimsPrincipal user);

        public Task Delete(int postId, ClaimsPrincipal user);

        public IQueryable<PostPreviewViewModel> GetAllPostsSortedBy(int sortType, int sortOrder, string searchTerm, string category);

        public string GenerateShortDescription(string escapedHtml);

        public Task<ViewPostViewModel> GenerateViewPostModelAsync(int postId);

        public Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId);

        public Task<EditPostFormModel> GenerateEditPostFormModelAsync(int postId, ClaimsPrincipal user);

        public Task<bool> PostExistsAsync(string postTitle);

        public Task<bool> PostExistsAsync(int postId);
    }
}