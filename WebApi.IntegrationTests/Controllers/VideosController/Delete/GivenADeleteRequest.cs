using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using WebApi.Models.Videos;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.VideosController.Delete
{
    [Collection(nameof(TestCollection))]
    public class GivenADeleteRequest : IClassFixture<GivenADeleteRequest.DeleteRequest>
    {
        public class DeleteRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Video Video { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public DeleteRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Video = VideoBuilder.CreateVideo("Cool Removable Video").Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertVideoCommand(Video));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .DeleteAsync($"/api/videos/{Video.VideoId}");
            }

            public async Task<Video> LoadStoredEvent()
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

        private readonly DeleteRequest _fixture;

        public GivenADeleteRequest(DeleteRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        [Fact]
        public async Task ThenTheVideoShouldNotExist() =>
            (await _fixture.LoadStoredEvent()).Should().BeNull();
    }
}
