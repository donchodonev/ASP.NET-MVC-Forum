namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class PostValidationService : IPostValidationService
    {
        private readonly IPostRepository postRepo;
        private readonly IHtmlManipulator htmlManipulator;
        private readonly ApplicationDbContext db;

        public PostValidationService(
            IPostRepository postRepo,
            IHtmlManipulator htmlManipulator,
            ApplicationDbContext db
)
        {
            this.postRepo = postRepo;
            this.htmlManipulator = htmlManipulator;
            this.db = db;
        }

        public void ValidatePostModelNotNull<T>(T post)
        {
            if (post == null)
            {
                throw new PostNullReferenceException(POST_DOES_NOT_EXIST);
            }
        }

        public async Task ValidatePostChangedAsync(int originalPostId, string newHtmlContent, string newTitle, int newCategoryId)
        {
            var originalPost = await postRepo.GetByIdAsync(originalPostId);

            var kvp = new Dictionary<string, bool>();

            var sanitizedAndDecodedHtml = htmlManipulator
                .Decode(htmlManipulator.Sanitize(newHtmlContent));

            if (originalPost.HtmlContent.Length != sanitizedAndDecodedHtml.Length)
            {
                kvp.Add("HtmlContent", true);
            }

            if (originalPost.Title != newTitle)
            {
                kvp.Add("Title", true);
            }

            if (originalPost.CategoryId != newCategoryId)
            {
                kvp.Add("CategoryId", true);
            }

            if (kvp.Keys.Count == 0)
            {
                throw new NoUpdatesMadeException(POST_DID_NOT_CHANGE);
            }
        }

        public async Task ValidatePostExistsAsync(int postId)
        {
            if(await db.Posts.AnyAsync(x => x.Id == postId))
            {
                throw new EntityDoesNotExistException(POST_DOES_NOT_EXIST);
            }
        }
    }
}
