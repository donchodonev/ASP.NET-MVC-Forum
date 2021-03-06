namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using System.Threading.Tasks;

    public interface IPostValidationService
    {
        public void ValidateNotNull<T>(T post);

        public Task ValidatePostChangedAsync(int originalPostId, string newHtmlContent, string newTitle, int newCategoryId);

        public Task ValidatePostExistsAsync(int postId);

        public Task ValidateTitleNotDuplicateAsync(string title);
    }
}
