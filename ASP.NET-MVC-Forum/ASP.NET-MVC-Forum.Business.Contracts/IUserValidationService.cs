﻿namespace ASP.NET_MVC_Forum.Business.Contracts.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IUserValidationService
    {
        public Task ValidateUserIsPrivilegedAsync(int postId, ClaimsPrincipal user);

        public void ValidateUsername(string username);

        public void ValidateUserNotNull(ExtendedIdentityUser user);

        public Task ValidateUserExistsByUsernameAsync(string username);

        public Task ValidateUserExistsByIdAsync(string userId);

        public Task ValidateUserIsNotBannedAsync(string userId);

        public Task ValidateUserIsBannedAsync(string userId);

        public Task ValidateUserIsNotModerator(ExtendedIdentityUser user);

        public Task ValidateUserIsModerator(string userId);
    }
}