using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.S3.Model;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.ImagesController.Post
{
    [Collection(nameof(TestCollection))]
    public class GivenAPostRequest : IClassFixture<GivenAPostRequest.PostRequest>
    {
        public class PostRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private ListObjectsV2Response _listObjectsResponse;
            public HttpResponseMessage Response { get; private set; }
            public int ImageCount => _listObjectsResponse.KeyCount;

            public PostRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                const string parameterName = "file";
                const string fileName = "1x1.gif";
                using (var imageStream = GetImageStream(fileName))
                using (var content = new MultipartFormDataContent
                {
                    Headers =
                    {
                        ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = parameterName,
                            FileName = fileName
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
                    }, parameterName, fileName);
                    Response = await _factory.Client.PostAsync("/api/images", content);
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

        private readonly PostRequest _fixture;

        public GivenAPostRequest(PostRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.Created);

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

        [Fact]
        public void ThenTheEventShouldBeStored() =>
            _fixture.ImageCount.Should().Be(1);
    }
}
