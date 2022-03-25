namespace ASP.NET_MVC_Forum.Data.Models
{
    using ASP.NET_MVC_Forum.Domain.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Chat : ICreatedOn
    {
        public Chat()
        {
            CreatedOn = DateTime.UtcNow;
            Messages = new HashSet<Message>();
        }

        [Required]
        public long Id { get; set; }

        [Required]
        public int UserA { get; set; }

        [Required]
        public int UserB { get; set; }

        public DateTime CreatedOn { get; init; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}