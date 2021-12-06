namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Models.Post;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostBusinessService
    {
        public Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel post, string identityUserId);

        public Task<Post> Edit(EditPostFormModel viewModelData);

        public Task<bool> IsAuthor(int userId, int postId);

        public Task<Dictionary<string, bool>> GetPostChangesAsync(int originalPostId, string newHtmlContent, string newTitle, int newCategoryId);

        public Task Delete(int postId);

        public IQueryable<PostPreviewViewModel> GetAllPostsSortedBy(int sortType, int sortOrder, string searchTerm, string category);

        public string GenerateShortDescription(string escapedHtml);

        public Task<ViewPostViewModel> GenerateViewPostModel(int postId);

        public Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId);

        public EditPostFormModel GenerateEditPostFormModel(int postId);
    }
}