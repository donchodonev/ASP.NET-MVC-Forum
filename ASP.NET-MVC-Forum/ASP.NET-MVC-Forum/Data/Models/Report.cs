namespace ASP.NET_MVC_Forum.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.ReportConstants;

    public class Report : BaseModel
    {
        public Report()
            :base()
        {

        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(ReportReasonMinLength)]
        [MaxLength(ReportReasonMaxLength)]
        public string Reason { get; set; }

        [Required]
        public int PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}
