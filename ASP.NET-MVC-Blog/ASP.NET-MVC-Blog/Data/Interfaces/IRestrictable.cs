namespace ASP.NET_MVC_Blog.Data.Interfaces
{
    using System;

    public interface IRestrictable
    {
        public bool IsBanned { get; set; }

        public DateTime? BannedUntil { get; set; }
    }
}
