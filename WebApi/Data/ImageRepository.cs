using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
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

        public ReadModelResult GetImageAsync(Guid imageId)
        {
            if (imageId == Guid.Empty)
                return new ReadModelResult(ResultStatus.Failed, "ImageId is not valid.");

            var modelResult = new ReadModelResult(imageId);

            try
            {
                var getPreSignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _configuration.GetSection("S3Buckets")["Images"],
                    Key = imageId.ToString(),
                    Expires = DateTime.MaxValue
                };

                var preSignedUrl = _amazonS3Client.GetPreSignedURL(getPreSignedUrlRequest);

                var imageLocation = new UriBuilder(preSignedUrl)
                {
                    Scheme = HttpScheme.Http.ToString()
                };

                modelResult.Locations = new[] { imageLocation.Uri.AbsoluteUri };
            }
            catch (Exception ex)
            {
                return new ReadModelResult(ResultStatus.Failed, ex.Message);
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
                var dataLength = (int)modelResult.Data.Length;
                var fileBytes = new byte[dataLength];
                modelResult.Data.OpenReadStream().Read(fileBytes, 0, dataLength);

                using (var stream = new MemoryStream(fileBytes))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _configuration.GetSection("S3Buckets")["Images"],
                        Key = modelResult.ImageId.ToString(),
                        InputStream = stream,
                        ContentType = modelResult.Data.ContentType,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    await _amazonS3Client.PutObjectAsync(request);
                }
            }
            catch (Exception ex)
            {
                return new CreateModelResult<IFormFile>(ResultStatus.Failed, ex.Message);
            }

            return modelResult;
        }

        public async Task<DeleteModelResult> DeleteImageAsync(Guid imageId)
        {
            if (imageId == Guid.Empty)
                return new DeleteModelResult(ResultStatus.Failed, "ImageId is not valid.");

            var modelResult = new DeleteModelResult(imageId);

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

        private static bool IsValidType(string contentType)
        {
            switch (contentType)
            {
                case "image/png":
                case "image/gif":
                case "image/jpeg":
                case "image/bmp":
                case "image/tiff":
                    return true;
                default:
                    return false;
            }
        }
    }
}
