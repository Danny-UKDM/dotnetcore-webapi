using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data.Writers
{
    public class ImageRepository : IImageRepository
    {
        private readonly AmazonS3Client _amazonS3Client;

        public ImageRepository()
        {
            var amazonS3Config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            };

            _amazonS3Client = new AmazonS3Client(amazonS3Config);
        }

        public Task<ModelResult<IFormFile>> GetImageAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public async Task<ModelResult<IFormFile>> SaveImageAsync(IFormFile file)
        {
            var modelResult = ValidateImageFile(file);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            try
            {
                string result;
                using (var reader = new StreamReader(modelResult.Data.OpenReadStream()))
                {
                    result = await reader.ReadToEndAsync();
                }

                var request = new PutObjectRequest
                {
                    BucketName = "imagesbucket",
                    Key = modelResult.ImageId.ToString(),
                    ContentBody = result
                };

                await _amazonS3Client.PutObjectAsync(request);

            }
            catch (Exception ex)
            {
                return new ModelResult<IFormFile>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }

        public Task<ModelResult<IFormFile>> DeleteImageAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        private static ModelResult<IFormFile> ValidateImageFile(IFormFile file)
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
    }
}
