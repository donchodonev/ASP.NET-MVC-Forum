namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Models.User
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;

    public class UserViewModel
    {
        public int Id { get; set; }

        public string Username => IdentityUser.UserName;

        public List<string> Roles { get; set; }

        public bool IsBanned { get; set; }

        public DateTime CreatedOn { get; set; }

        public IdentityUser IdentityUser { get; set; }
    }
}
