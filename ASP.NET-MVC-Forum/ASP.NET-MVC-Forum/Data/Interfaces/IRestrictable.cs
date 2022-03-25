namespace ASP.NET_MVC_Forum.Web.Data.Interfaces
{
    using System;

    public interface IRestrictable
    {
        public bool IsBanned { get; set; }

        public DateTime? BannedUntil { get; set; }
    }
}
