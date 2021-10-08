namespace ASP.NET_MVC_Blog.Data.Models
{
    using ASP.NET_MVC_Blog.Data.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static ASP.NET_MVC_Blog.Data.DataConstants.UserConstants;

    [Index(nameof(Id))]
    public class User : BaseModel
    {
        public User()
            : base()
        {
            Posts = new HashSet<Post>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        [MinLength(FirstNameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        [MinLength(FirstNameMinLength)]
        public string LastName { get; set; }

        [Range(AgeFloor, AgeCeiling)]
        public int? Age { get; set; }

        public bool? Gender { get; set; } //true for Male, false for Female

        public bool IsBanned { get; set; }

        public DateTime? BannedUntil { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required]
        public virtual IdentityUser IdentityUser { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
