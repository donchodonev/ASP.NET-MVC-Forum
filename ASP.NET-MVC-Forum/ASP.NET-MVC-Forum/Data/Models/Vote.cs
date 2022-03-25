using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MVC_Forum.Web.Data.Models
{
    public class Vote : BaseModel
    {
        public Vote()
            :base()
        {
        }

        [Required]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public VoteType VoteType { get; set; }
    }
}
