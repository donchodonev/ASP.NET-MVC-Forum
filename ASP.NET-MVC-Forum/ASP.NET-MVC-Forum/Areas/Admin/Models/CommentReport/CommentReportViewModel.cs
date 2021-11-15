namespace ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport
{
    using System;

    public class CommentReportViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ShortReason { get; set; }

        public string Reason { get; set; }

        public int CommentId { get; set; }

        public string CommentContent { get; set; }

        public string CreatedOnAsString
        {
            get
            {
                string returnString;

                DateTime currentTime = DateTime.UtcNow;
                var differenceInSeconds = (currentTime - CreatedOn).Seconds;
                var differenceInMinutes = (currentTime - CreatedOn).Minutes;
                var differenceInHours = (currentTime - CreatedOn).Hours;
                var differenceInDays = (currentTime - CreatedOn).Days;

                if (differenceInDays > 1)
                {
                    returnString = $"{differenceInDays} days ago";
                }
                else if (differenceInDays == 1)
                {
                    returnString = "1 day ago";
                }
                else if (differenceInHours <= 23 && differenceInHours > 1)
                {
                    returnString = $"{differenceInHours} hours ago";
                }
                else if (differenceInHours == 1)
                {
                    returnString = "1 hour ago";
                }
                else if (differenceInMinutes <= 59 && differenceInMinutes > 1)
                {
                    returnString = $"{differenceInMinutes} minutes ago";
                }
                else if (differenceInMinutes == 1)
                {
                    returnString = "1 minute ago";
                }
                else if (differenceInSeconds <= 59 && differenceInSeconds > 1)
                {
                    returnString = $"{differenceInSeconds} seconds ago";
                }
                else
                {
                    returnString = $"1 second ago";
                }

                return returnString;
            }
        }

        public bool IsDeleted { get; set; }
    }
}
