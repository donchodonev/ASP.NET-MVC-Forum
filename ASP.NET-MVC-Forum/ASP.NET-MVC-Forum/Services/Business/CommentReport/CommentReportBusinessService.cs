using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
using ASP.NET_MVC_Forum.Services.Data.CommentReport;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.CommentReport
{
    public class CommentReportBusinessService : ICommentReportBusinessService
    {
        private readonly IMapper mapper;
        private readonly ICommentReportDataService data;

        public CommentReportBusinessService(IMapper mapper, ICommentReportDataService data)
        {
            this.mapper = mapper;
            this.data = data;
        }
        public async Task<List<CommentReportViewModel>> GenerateCommentReportViewModelListAsync(string reportStatus)
        {
            if (reportStatus == "Active")
            {
                return await mapper.ProjectTo<CommentReportViewModel>(data.All()).ToListAsync();
            }

            return await mapper.ProjectTo<CommentReportViewModel>(data.All(isDeleted: true)).ToListAsync();
        }
    }
}
