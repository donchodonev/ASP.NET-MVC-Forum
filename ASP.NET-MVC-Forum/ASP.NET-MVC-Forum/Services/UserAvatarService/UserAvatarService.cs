
namespace ASP.NET_MVC_Forum.Services.UserAvatarService
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.DataConstants.ImageConstants;
    using static ASP.NET_MVC_Forum.Data.DataConstants.WebConstants;
    public class UserAvatarService : IUserAvatarService
    {
        private readonly IWebHostEnvironment enviroment;

        public UserAvatarService(IWebHostEnvironment enviroment)
        {
            this.enviroment = enviroment;
        }

        public string GetImageExtension(IFormFile image)
        {
            string[] validFileExtensions = new string[5] { JPG, JPEG, PNG, WEBP, BMP };

            string imageExtensionName = null;

            foreach (var currentFileExtension in validFileExtensions)
            {
                if (image.FileName.EndsWith(currentFileExtension))
                {
                    imageExtensionName = currentFileExtension;
                    break;
                }
            }

            return imageExtensionName;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, int width = 50, int height = 50)
        {
            if (!IsImageSizeValid(file.Length))
            {
                throw new ArgumentOutOfRangeException("The image must be up to 5 MB (megabytes) in size");
            }

            string imageExtension = GetImageExtension(file);

            if (imageExtension == null)
            {
                string[] allowedFileExtensions = new string[5] { JPG, JPEG, PNG, WEBP, BMP };
                throw new ArgumentOutOfRangeException($"The allowed image file formats are {string.Join(' ', allowedFileExtensions)}");
            }

            string guid = Guid.NewGuid().ToString();

            var fileName = $"{guid}{imageExtension}";

            string root = enviroment.ContentRootPath;

            string fullPath = $"{root}{AvatarDirectoryPath}{fileName}";

            using (Image image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(width, height));
                await image.SaveAsync(fullPath);
            }

            return fileName;
        }

        public bool IsImageSizeValid(long imageSize, long maxImageSize = ImageMaxSize)
        {
            return imageSize <= maxImageSize;
        }
    }
}
