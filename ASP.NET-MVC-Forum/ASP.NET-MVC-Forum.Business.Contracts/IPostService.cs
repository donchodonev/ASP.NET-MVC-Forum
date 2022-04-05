namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<bool> IsUserPrivileged(int postId, ClaimsPrincipal currentPrincipal);

        public Task<AddPostFormModel> GeneratedAddPostFormModelAsync();

        public Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel post);

        public Task<Post> Edit(EditPostFormModel viewModelData);

        public Task<bool> IsAuthor(string userId, int postId);

        public Task Delete(int postId);

        public IQueryable<PostPreviewViewModel> GetAllPostsSortedBy(int sortType, int sortOrder, string searchTerm, string category);

        public string GenerateShortDescription(string escapedHtml);

        public Task<ViewPostViewModel> GenerateViewPostModelAsync(int postId);

        public Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId);

        public Task<EditPostFormModel> GenerateEditPostFormModelAsync(int postId);

        public Task<bool> PostExistsAsync(string postTitle);

        public Task<bool> PostExistsAsync(int postId);
    }
}