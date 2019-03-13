using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using WebApi.Models;

namespace WebApi.Data
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

        public async Task<ModelResult<byte[]>> GetImageAsync(string imageId)
        {
            var modelResult = ValidateImageRequest(imageId);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = "imagesbucket",
                    Key = imageId
                };

                var getObjectResponse = await _amazonS3Client.GetObjectAsync(request);

                using (var responseStream = getObjectResponse.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                using (var memoryStream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memoryStream);
                    modelResult.Data = memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                return new ModelResult<byte[]>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
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

                modelResult.ImageId = string.Concat(Guid.NewGuid(), Path.GetFileName(file.FileName));

                var request = new PutObjectRequest
                {
                    BucketName = "imagesbucket",
                    Key = modelResult.ImageId,
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

        public Task<ModelResult<IFormFile>> DeleteImageAsync(string key)
        {
            throw new NotImplementedException();
        }

        private static ModelResult<IFormFile> ValidateImageFile(IFormFile file)
        {
            if (file.Length == 0)
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image size is zero.");

            if (file.Length >= 5e+7)
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image size is too large.");

            if (!IsValidType(file.ContentType))
                return new ModelResult<IFormFile>(ResultStatus.Failed, "Image is not a valid type.");

            return new ModelResult<IFormFile>(file);
        }

        private static ModelResult<byte[]> ValidateImageRequest(string imageId)
        {
            if (imageId.Length == 0)
                return new ModelResult<byte[]>(ResultStatus.Failed, "ImageId is empty.");

            var contentType = ResolveContentTypeFromKey(imageId);
            if (string.IsNullOrEmpty(contentType))
                return new ModelResult<byte[]>(ResultStatus.Failed, "Failed to resolve content type from ImageId.");

            return new ModelResult<byte[]>(imageId, contentType);
        }

        private static string ResolveContentTypeFromKey(string key)
        {
            var extension = Path.GetExtension(key);

            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                default:
                    return "";
            }
        }

        private static bool IsValidType(string contentType)
        {
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
