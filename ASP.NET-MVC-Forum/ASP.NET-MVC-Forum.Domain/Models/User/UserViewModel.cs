namespace ASP.NET_MVC_Forum.Domain.Models.User
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System;
    using System.Collections.Generic;

    public class UserViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public List<string> Roles { get; set; }

        public bool IsBanned { get; set; }

        public DateTime CreatedOn { get; set; }

        public ExtendedIdentityUser IdentityUser { get; set; }
    }
}
