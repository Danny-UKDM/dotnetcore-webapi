using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using WebApi.Models.Videos;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.VideosController.Post
{
    [Collection(nameof(TestCollection))]
    public class GivenAPostRequest : IClassFixture<GivenAPostRequest.PostRequest>
    {
        public class PostRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Video Video { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public PostRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Video = VideoBuilder.CreateVideo("Nice Test Video").Build();

                var httpContent = new ObjectContent<Video>(Video, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.HttpClient
                                         .PostAsync("/api/videos", httpContent);
            }

            public async Task<Video> LoadStoredVideo()
            {
                using (var session = _factory.SessionFactory.CreateQuerySession())
                {
                    return await session.ExecuteAsync(new GetVideoByIdQuery(Video.VideoId));
                }
            }

            public async Task DisposeAsync()
            {
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteRowsByVideoIdCommand(new[] { Video.VideoId }));
                    session.Commit();
                }
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
        public async Task ThenTheVideoShouldBeStored() =>
            (await _fixture.LoadStoredVideo()).Should().BeEquivalentTo(_fixture.Video);

    }
}
