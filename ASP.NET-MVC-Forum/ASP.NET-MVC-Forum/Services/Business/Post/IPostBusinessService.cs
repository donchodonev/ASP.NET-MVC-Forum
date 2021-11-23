namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;

    public interface IPostBusinessService
    {
        public Task<int> CreateNew(Post post, int userId);
    }
}