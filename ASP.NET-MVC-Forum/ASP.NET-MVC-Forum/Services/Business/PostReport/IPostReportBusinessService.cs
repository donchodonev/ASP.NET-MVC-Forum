﻿namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using System.Threading.Tasks;

    public interface IPostReportBusinessService
    {
        public Task Delete(int id);

        public Task Restore(int id);

        public Task AutoGeneratePostReport(string title, string content, int postId);
    }
}
