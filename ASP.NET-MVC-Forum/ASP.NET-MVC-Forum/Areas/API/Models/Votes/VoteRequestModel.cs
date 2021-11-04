﻿using System.ComponentModel.DataAnnotations;

namespace ASP.NET_MVC_Forum.Areas.API.Models.Votes
{
    public class VoteRequestModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public bool IsPositiveVote { get; set; }
    }
}