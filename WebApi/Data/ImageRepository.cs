using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly IConfiguration _configuration;
        private readonly AmazonS3Client _amazonS3Client;
        public ImageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            var amazonS3Config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4572",
                AuthenticationRegion = "eu-west-1",
                ForcePathStyle = true
            };

            _amazonS3Client = new AmazonS3Client(amazonS3Config);
        }

        public async Task<ReadModelResult<byte[]>> GetImageAsync(Guid imageId)
        {
            var modelResult = ValidateReadRequest(imageId);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _configuration.GetSection("S3Buckets")["Images"],
                    Key = imageId.ToString()
                };

                var getObjectResponse = await _amazonS3Client.GetObjectAsync(request);
                modelResult.ContentType = getObjectResponse.Headers.ContentType;

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
                return new ReadModelResult<byte[]>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }

        public async Task<CreateModelResult<IFormFile>> SaveImageAsync(IFormFile file, Guid imageId = default)
        {
            var modelResult = ValidateCreateRequest(file);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            modelResult.ImageId = imageId == default
                ? Guid.NewGuid()
                : imageId;

            try
            {
                string imageString;
                using (var reader = new StreamReader(modelResult.Data.OpenReadStream()))
                {
                    imageString = await reader.ReadToEndAsync();
                }

                var request = new PutObjectRequest
                {
                    BucketName = _configuration.GetSection("S3Buckets")["Images"],
                    Key = modelResult.ImageId.ToString(),
                    ContentBody = imageString,
                    ContentType = file.ContentType
                };

                await _amazonS3Client.PutObjectAsync(request);

            }
            catch (Exception ex)
            {
                return new CreateModelResult<IFormFile>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }

        public async Task<DeleteModelResult> DeleteImageAsync(Guid imageId)
        {
            var modelResult = ValidateDeleteRequest(imageId);

            if (modelResult.Result == ResultStatus.Failed)
                return modelResult;

            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _configuration.GetSection("S3Buckets")["Images"],
                    Key = modelResult.ImageId.ToString()
                };

                await _amazonS3Client.DeleteObjectAsync(request);
            }
            catch (Exception ex)
            {
                return new DeleteModelResult(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }

        private static CreateModelResult<IFormFile> ValidateCreateRequest(IFormFile file)
        {
            if (file.Length == 0)
                return new CreateModelResult<IFormFile>(ResultStatus.Failed, "Image size is zero.");

            if (file.Length >= 5e+7)
                return new CreateModelResult<IFormFile>(ResultStatus.Failed, "Image size is too large.");

            if (!IsValidType(file.ContentType))
                return new CreateModelResult<IFormFile>(ResultStatus.Failed, "Image is not a valid type.");

            return new CreateModelResult<IFormFile>(file);
        }

        private static ReadModelResult<byte[]> ValidateReadRequest(Guid imageId)
        {
            return imageId != Guid.Empty
                ? new ReadModelResult<byte[]>(imageId)
                : new ReadModelResult<byte[]>(ResultStatus.Failed, "ImageId is not valid.");
        }

        private static DeleteModelResult ValidateDeleteRequest(Guid imageId)
        {
            return imageId != Guid.Empty
                ? new DeleteModelResult(imageId)
                : new DeleteModelResult(ResultStatus.Failed, "ImageId is not valid.");
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
