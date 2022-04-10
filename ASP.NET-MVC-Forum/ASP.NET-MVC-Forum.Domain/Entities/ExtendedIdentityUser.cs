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
            CreatedOn = DateTime.UtcNow;
        }

        [Required]
        [MaxLength(FIRST_NAME_MAX_LENGTH)]
        [MinLength(FIRST_NAME_MIN_LENGTH)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(LAST_NAME_MAX_LENGTH)]
        [MinLength(LAST_NAME_MIN_LENGTH)]
        public string LastName { get; set; }

        [Range(AGE_FLOOR, AGE_CEILING)]
        public int? Age { get; set; }

        public bool IsBanned { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? BannedUntil { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get ; set ; }

        public bool IsDeleted { get ; set ; }
    }
}
