using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.VideosController.Get
{
    [Collection(nameof(TestCollection))]
    public class GivenAGetRequest : IClassFixture<GivenAGetRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Video Video { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Video = VideoBuilder.CreateVideo("Cool Test Video").Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertVideoCommand(Video));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .GetAsync($"/api/videos/{Video.VideoId}");
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

        private readonly GetRequest _fixture;

        public GivenAGetRequest(GetRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.OK);

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

        [Fact]
        public async Task ThenTheResponseContentHasExpectedValue() =>
            (await _fixture.Response.Content.ReadAsAsync<Video>()).Should().BeEquivalentTo(_fixture.Video);
    }
}
