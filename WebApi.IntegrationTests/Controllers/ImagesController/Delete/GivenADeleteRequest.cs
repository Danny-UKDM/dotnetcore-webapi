using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.ImagesController.Delete
{
    [Collection(nameof(TestCollection))]
    public class GivenADeleteRequest : IClassFixture<GivenADeleteRequest.DeleteRequest>
    {
        public class DeleteRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private ListObjectsV2Response _listObjectsResponse;
            private readonly Guid _imageKey = Guid.NewGuid();
            private const string FileName = "1x1.gif";
            public HttpResponseMessage Response { get; private set; }
            public int ImageCount => _listObjectsResponse.KeyCount;

            public DeleteRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                string imageString;
                using (var imageStream = GetImageStream(FileName))
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

                Response = await _factory.HttpClient.DeleteAsync($"/api/images/{_imageKey}");

                _listObjectsResponse = await _factory.AmazonS3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = _factory.ImageBucketName });
            }

            private static FileStream GetImageStream(string fileName) =>
                File.OpenRead($"../../../Controllers/ImagesController/Images/{fileName}");

            public async Task DisposeAsync()
            {
                if (ImageCount != 0)
                {
                    await _factory.AmazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                    {
                        BucketName = _factory.ImageBucketName,
                        Objects = _listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                    }); 
                }
            }
        }

        private readonly DeleteRequest _fixture;

        public GivenADeleteRequest(DeleteRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        [Fact]
        public void ThenTheImageShouldBeDeleted() =>
            _fixture.ImageCount.Should().Be(0);
    }
}
