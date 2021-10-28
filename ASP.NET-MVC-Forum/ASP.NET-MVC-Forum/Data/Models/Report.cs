﻿namespace ASP.NET_MVC_Forum.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
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

        [NotMapped]
        public string ShortReason
        {
            get
            {
                if (Reason.Count() <= 70)
                {
                    return Reason;
                }

                return Reason.Substring(0,70) + "...";
            }
        }

        [Required]
        public int PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}
