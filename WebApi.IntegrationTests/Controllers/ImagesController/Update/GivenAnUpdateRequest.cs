using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.ImagesController.Update
{
    [Collection(nameof(TestCollection))]
    public class GivenAnUpdateRequest : IClassFixture<GivenAnUpdateRequest.UpdateRequest>
    {
        public class UpdateRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private readonly Guid _imageKey = Guid.NewGuid();
            private const string ExistingFileName = "1x1.gif";
            private const string UpdatedFileName = "1x1.png";
            private ListObjectsV2Response _listObjectsResponse;
            public HttpResponseMessage Response { get; private set; }
            public int ImageCount => _listObjectsResponse.KeyCount;

            public UpdateRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                string imageString;
                using (var imageStream = GetImageStream(ExistingFileName))
                using (var reader = new StreamReader(imageStream))
                {
                    imageString = await reader.ReadToEndAsync();
                }

                var request = new PutObjectRequest
                {
                    BucketName = _factory.ImageBucketName,
                    Key = _imageKey.ToString(),
                    ContentBody = imageString,
                    ContentType = "image/gif"
                };

                await _factory.AmazonS3Client.PutObjectAsync(request);



                const string parameterName = "file";

                using (var imageStream = GetImageStream(UpdatedFileName))
                using (var content = new MultipartFormDataContent
                {
                    Headers =
                    {
                        ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = parameterName,
                            FileName = UpdatedFileName
                        }
                    }
                })
                {
                    content.Add(new StreamContent(imageStream)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("image/gif")
                        }
                    }, parameterName, UpdatedFileName);

                    Response = await _factory.HttpClient.PutAsync($"/api/images/{_imageKey}", content);
                }

                _listObjectsResponse = await _factory.AmazonS3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _factory.ImageBucketName });
            }

            private static FileStream GetImageStream(string fileName) =>
                File.OpenRead($"../../../Controllers/ImagesController/Images/{fileName}");

            public async Task DisposeAsync()
            {
                await _factory.AmazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = _factory.ImageBucketName,
                    Objects = _listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                });
            }
        }

        private readonly UpdateRequest _fixture;

        public GivenAnUpdateRequest(UpdateRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        [Fact]
        public void TheTheExistingImageShouldBeReplaced() =>
            _fixture.ImageCount.Should().Be(1);
    }
}
