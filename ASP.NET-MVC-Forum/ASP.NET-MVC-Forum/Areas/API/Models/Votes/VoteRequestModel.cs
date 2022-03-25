namespace ASP.NET_MVC_Forum.Web.Areas.API.Models.Votes
{
    using System.ComponentModel.DataAnnotations;

    public class VoteRequestModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public bool IsPositiveVote { get; set; }
    }
}
