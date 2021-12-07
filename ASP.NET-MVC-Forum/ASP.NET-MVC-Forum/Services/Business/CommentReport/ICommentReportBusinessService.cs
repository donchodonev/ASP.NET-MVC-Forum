using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.CommentReport
{
    public interface ICommentReportBusinessService
    {
        public Task<List<CommentReportViewModel>> GenerateCommentReportViewModelListAsync(string reportStatus);
    }
}
