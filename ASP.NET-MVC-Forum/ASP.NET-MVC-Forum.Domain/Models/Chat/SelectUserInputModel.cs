namespace ASP.NET_MVC_Forum.Domain.Models.Chat
{
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.UserConstants;

    public class SelectUserInputModel
    {
        [Required]
        [StringLength(
            USERNAME_MAX_LENGTH,
            MinimumLength = USERNAME_MIN_LENGTH,
            ErrorMessage = USERNAME_TOO_SHORT)]
        public string Username { get; set; }
    }
}
