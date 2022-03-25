namespace ASP.NET_MVC_Forum.Domain.Interfaces
{
    using System;

    public interface IRestrictable
    {
        public bool IsBanned { get; set; }

        public DateTime? BannedUntil { get; set; }
    }
}
