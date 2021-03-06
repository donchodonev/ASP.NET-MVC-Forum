namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ImageConstants;
    using static ASP.NET_MVC_Forum.Domain.Constants.WebConstants;

    public class AvatarRepository : IAvatarRepository
    {
        private readonly IWebHostEnvironment enviroment;
        private readonly string[] validFileExtensions;

        public AvatarRepository(IWebHostEnvironment enviroment)
        {
            this.enviroment = enviroment;
            validFileExtensions = new string[4] { JPG, JPEG, PNG, BMP};
        }

        public string GetImageExtension(IFormFile image)
        {
            return validFileExtensions.FirstOrDefault(x => image.FileName.ToLower().EndsWith(x));
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, int width = 50, int height = 50)
        {
            if (file == null)
            {
                throw new ArgumentOutOfRangeException("Please choose an image to upload before using the \"Upload\" button");
            }

            if (!IsImageSizeValid(file.Length))
            {
                throw new ArgumentOutOfRangeException("The image must be up to 5 MB (megabytes) in size");
            }

            string imageExtension = GetImageExtension(file);

            if (imageExtension == null)
            {
                throw new ArgumentOutOfRangeException($"The allowed image file formats are {string.Join(' ', validFileExtensions)}");
            }

            string guid = Guid.NewGuid().ToString();

            var fileName = $"{guid}{imageExtension}";

            string root = enviroment.ContentRootPath;

            string fullPath = $"{root}{AVATAR_DIRECTORY_PATH}{fileName}";

            using (Image image = Image.Load(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(width, height));

                await image.SaveAsync(fullPath);
            }

            return fileName;
        }

        public bool IsImageSizeValid(long imageSize, long maxImageSize = IMAGE_MAX_SIZE)
        {
            return imageSize <= maxImageSize;
        }
    }
}
