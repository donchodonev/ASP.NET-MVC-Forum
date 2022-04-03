using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MVC_Forum.Domain.Entities
{
    public class Vote : BaseModel
    {
        public Vote()
            :base()
        {
        }

        [Required]
        public int Id { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public string UserId { get; set; }

        public virtual ExtendedIdentityUser User { get; set; }

        [Required]
        public VoteType VoteType { get; set; }
    }
}
