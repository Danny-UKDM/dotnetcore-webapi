using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public class ImageWriter : IImageWriter
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageWriter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ModelResult<IFormFile>> UploadImage(IFormFile file)
        {
            var modelResult = ValidateImageFile(file);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            return await WriteImageFile(modelResult);
        }

        private ModelResult<IFormFile> ValidateImageFile(IFormFile file)
        {
            if (file.Length == 0)
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image size is zero.");

            if (file.Length >= 5e+7)
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image size is too large.");

            if (!IsValidType(file))
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image is not a valid type.");

            return new ModelResult<IFormFile>(file);
        }

        private static bool IsValidType(IFormFile file)
        {
            var contentType = file.ContentType;

            switch (contentType)
            {
                case "image/png":
                case "image/gif":
                case "image/jpeg":
                    return true;
                default:
                    return false;
            }
        }

        private async Task<ModelResult<IFormFile>> WriteImageFile(ModelResult<IFormFile> modelResult)
        {
            try
            {
                var extension = "." + modelResult.Data.ContentType.Split('/')[modelResult.Data.ContentType.Split('/').Length - 1];
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "images", modelResult.ImageId + extension);

                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await modelResult.Data.CopyToAsync(bits);
                }
            }
            catch (Exception ex)
            {
                return new ModelResult<IFormFile>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }
    }
}
