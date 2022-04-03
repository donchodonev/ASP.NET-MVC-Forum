namespace ASP.NET_MVC_Forum.Domain.Entities
{
    using ASP.NET_MVC_Forum.Domain.Interfaces;

    using Microsoft.AspNetCore.Identity;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants.UserConstants;

    public class ExtendedIdentityUser : IdentityUser, IDeletable, IMetaData, IContainImage
    {
        public ExtendedIdentityUser()
            : base()
        {
            Posts = new HashSet<Post>();
            Chats = new HashSet<Chat>();
            CreatedOn = DateTime.UtcNow;
        }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        [MinLength(FirstNameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(LastNameMaxLength)]
        [MinLength(LastNameMinLength)]
        public string LastName { get; set; }

        [Range(AgeFloor, AgeCeiling)]
        public int? Age { get; set; }

        public bool IsBanned { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? BannedUntil { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Chat> Chats { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get ; set ; }

        public bool IsDeleted { get ; set ; }
    }
}
