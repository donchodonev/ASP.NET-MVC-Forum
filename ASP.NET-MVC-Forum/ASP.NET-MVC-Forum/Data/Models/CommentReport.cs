using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ASP.NET_MVC_Forum.Data.Models
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
                if (Reason.Count() <= 70)
                {
                    return Reason;
                }

                return Reason.Substring(0, 70) + "...";
            }
        }

        [Required]
        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
