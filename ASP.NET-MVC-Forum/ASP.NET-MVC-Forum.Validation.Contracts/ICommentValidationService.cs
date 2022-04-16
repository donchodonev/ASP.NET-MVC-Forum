namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Threading.Tasks;

    public interface ICommentValidationService
    {
        public void ValidateCommentNotNull(Comment comment);

        public Task ValidateUserCanDeleteCommentAsync(int commentId,
            string userId,
            bool isInAdminOrModeratorRole);

        public Task ValidateCommentExistsAsync(int commentId);
    }
}
