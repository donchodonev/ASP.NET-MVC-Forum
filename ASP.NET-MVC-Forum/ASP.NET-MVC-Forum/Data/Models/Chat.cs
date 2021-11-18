namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Data.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Chat : ICreatedOn
    {
        public Chat()
        {
            CreatedOn = DateTime.UtcNow;
        }

        [Required]
        public long Id { get; set; }

        [Required]
        public int UserA { get; set; }

        [Required]
        public int UserB { get; set; }

        public DateTime CreatedOn { get; init; }
    }
}