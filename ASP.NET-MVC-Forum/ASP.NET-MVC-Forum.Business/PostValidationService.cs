namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;
    
    public class PostValidationService :IPostValidationService
    {
        public void ValidatePostNotNull(Post post)
        {
            if (post == null)
            {
                throw new PostNullReferenceException(POST_DOES_NOT_EXIST);
            }
        }
    }
}
