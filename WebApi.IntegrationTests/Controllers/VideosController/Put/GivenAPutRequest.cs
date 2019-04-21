using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.VideosController.Put
{
    [Collection(nameof(TestCollection))]
    public class GivenAPutRequest : IClassFixture<GivenAPutRequest.PutRequest>
    {
        public class PutRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private Video _originalVideo;
            public Video UpdatedVideo { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public PutRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                _originalVideo = VideoBuilder.CreateVideo("Cool OG Video").Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    session.Execute(new InsertVideoCommand(_originalVideo));
                    session.Commit();
                }

                UpdatedVideo = VideoBuilder.CreateVideo("Cool New Video")
                                           .WithId(_originalVideo.VideoId)
                                           .Build();

                var httpContent = new ObjectContent<Video>(UpdatedVideo, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.HttpClient
                                         .PutAsync($"/api/videos/{_originalVideo.VideoId}", httpContent);
            }

            public async Task<Video> LoadStoredVideo()
            {
                using (var session = _factory.SessionFactory.CreateQuerySession())
                {
                    return await session.ExecuteAsync(new GetVideoByIdQuery(UpdatedVideo.VideoId));
                }
            }

            public async Task DisposeAsync()
            {
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteRowsByVideoIdCommand(new[] { _originalVideo.VideoId, UpdatedVideo.VideoId }));
                    session.Commit();
                }
            }
        }

        private readonly PutRequest _fixture;

        public GivenAPutRequest(PutRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        [Fact]
        public async Task ThenTheVideoShouldBeUpdated() =>
            (await _fixture.LoadStoredVideo()).Should().BeEquivalentTo(_fixture.UpdatedVideo);
    }
}
