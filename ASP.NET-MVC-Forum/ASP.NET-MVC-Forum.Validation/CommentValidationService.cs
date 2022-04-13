namespace ASP.NET_MVC_Forum.Validation
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using Microsoft.EntityFrameworkCore;

    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class CommentValidationService : ICommentValidationService
    {
        private readonly ICommentRepository commentRepo;

        public CommentValidationService(ICommentRepository commentRepo)
        {
            this.commentRepo = commentRepo;
        }

        public void ValidateCommentNotNull(Comment comment)
        {
            if (comment == null)
            {
                throw new NullCommentException(ENTITY_IS_NULL);
            }
        }

        public async Task ValidateUserCanDeleteCommentAsync(int commentId, ClaimsPrincipal user)
        {
            string userId = user.Id();

            bool canDelete = await commentRepo
                .All()
                .AnyAsync(x => x.Id == commentId 
                && (x.UserId == userId || user.IsAdminOrModerator()));

            if (!canDelete)
            {
                throw new InsufficientPrivilegeException(CANNOT_DELETE_COMMENT);
            }
        }

        public async Task ValidateCommentExistsAsync(int commentId)
        {
            if(! await commentRepo.ExistsAsync(commentId))
            {
                throw new EntityDoesNotExistException(ENTITY_DOES_NOT_EXIST);
            }
        }
    }
}
