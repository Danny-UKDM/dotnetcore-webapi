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

namespace WebApi.IntegrationTests.Controllers.ImagesController.Get
{
    [Collection(nameof(TestCollection))]
    public class GivenAGetRequest : IClassFixture<GivenAGetRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private readonly Guid _imageKey = Guid.NewGuid();
            private const string FileName = "1x1.gif";
            public HttpResponseMessage Response { get; private set; }
            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

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

                Response = await _factory.Client.GetAsync($"/api/images/{_imageKey}");
            }

            public string StoredTestImage()
            {
                string fileString;
                using (var imageStream = GetImageStream(FileName))
                {
                    var reader = new StreamReader(imageStream);
                    fileString = reader.ReadToEnd();
                }

                return fileString;
            }

            private static FileStream GetImageStream(string fileName) =>
                File.OpenRead($"../../../Controllers/ImagesController/Images/{fileName}");

            public async Task DisposeAsync()
            {
                var listObjectsResponse = await _factory.AmazonS3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = _factory.ImageBucketName
                });

                await _factory.AmazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = _factory.ImageBucketName,
                    Objects = listObjectsResponse.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                });
            }
        }

        private readonly GetRequest _fixture;

        public GivenAGetRequest(GetRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.OK);

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("image/gif"));

        [Fact]
        public async Task ThenTheResponseContentHasExpectedValue() =>
            (await _fixture.Response.Content.ReadAsStringAsync()).Should().Be(_fixture.StoredTestImage());

    }
}
