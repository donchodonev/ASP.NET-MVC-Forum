using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ASP.NET_MVC_Forum.Domain.Entities
{
    public class CommentReport : BaseModel
    {
        public int Id { get; set; }

        public string Reason { get; set; }

        [NotMapped]
        public string ShortReason
        {
            get
            {
                if (Reason.Count() <= 30)
                {
                    return Reason;
                }

                return Reason.Substring(0, 30) + "...";
            }
        }

        [Required]
        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
