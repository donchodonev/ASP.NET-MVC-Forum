﻿namespace ASP.NET_MVC_Forum.Web.Areas.API.Models.Stats
{
    public class MostReportedPostsResponeModel : IStatsResponseModel
    {
        public int Count { get; set; }

        public int Id { get; set; }

        public string Color { get; set; }

        public string Title { get; set; }
    }
}
