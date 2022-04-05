namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    public class PostValidationService :IPostValidationService
    {
        public void ValidatePostNotNull(Post post)
        {
            if (post == null)
            {

            }
        }
    }
}
