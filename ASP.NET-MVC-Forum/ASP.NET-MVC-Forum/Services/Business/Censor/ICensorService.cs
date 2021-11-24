namespace ASP.NET_MVC_Forum.Services.Business.Censor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICensorService
    {
        public bool ContainsProfanity(string term);

        public List<string> FindPostProfanities(string title, string content);

        public List<string> FindPostProfanities(string title, string content, string shortDescription);

        public Task CensorPostAsync(int postId);

        public Task CensorPostWithRegexAsync(int postId);
    }
}
