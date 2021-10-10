namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<IQueryable<Post>> AllAsync(bool withCategory = false); 
    }
}
