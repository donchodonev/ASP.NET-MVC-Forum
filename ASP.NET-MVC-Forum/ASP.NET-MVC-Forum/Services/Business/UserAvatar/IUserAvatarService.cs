namespace ASP.NET_MVC_Forum.Web.Services.Business.UserAvatar
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Web.Data.Constants.ImageConstants;

    public interface IUserAvatarService
    {
        /// <summary>
        /// Gets said image's extension if extension is amongst the allowed image extensions defined at ASP.NET_MVC_Forum.Data.DataConstants.AllowedImageExtensions that are used in the implementation of the GetImageExtension() method in UserAvatarService
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public string GetImageExtension(IFormFile image);


        /// <summary>
        /// Uploads user avatar image asynchronously
        /// </summary>
        /// <param name="file">IFormFile - the image</param>
        /// <param name="width">Desired image width - default is 50px</param>
        /// <param name="height">Desired image height - default is 50px</param>
        /// <returns></returns>
        public Task<string> UploadAvatarAsync(IFormFile file, int width = 50, int height = 50);

        public bool IsImageSizeValid(long imageSize, long maxImageSize = ImageMaxSize)
        {
            return imageSize <= maxImageSize;
        }
    }
}
