﻿namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface ICommentValidationService
    {
        public void ValidateCommentNotNull(Comment comment);

        public Task ValidateUserCanDeleteCommentAsync(int commentId, ClaimsPrincipal user);

        public Task ValidateCommentExistsAsync(int commentId);
    }
}
