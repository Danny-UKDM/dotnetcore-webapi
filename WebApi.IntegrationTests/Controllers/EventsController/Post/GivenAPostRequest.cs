using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Post
{
    [Collection(nameof(EventsCollection))]
    public class GivenAPostRequest : IClassFixture<GivenAPostRequest.PostRequest>
    {
        public class PostRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event Event { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public PostRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Event = EventBuilder.CreateEvent("Nice Test Event")
                                    .InCity("Nice Test City City")
                                    .Build();
                
                var httpContent = new ObjectContent<Event>(Event, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.Client
                                         .PostAsync("/api/events", httpContent);
            }

            public async Task<Event> LoadStoredEvent()
            {
                using (var session = _factory.SessionFactory.CreateQuerySession())
                {
                    return await session.ExecuteAsync(new GetEventByIdQuery(Event.EventId));
                }
            }

            public async Task DisposeAsync()
            {
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteRowsByEventIdCommand(new[] {Event.EventId}));
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
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") {CharSet = "utf-8"});

        [Fact]
        public async Task ThenTheEventShouldBeStored() =>
            (await _fixture.LoadStoredEvent()).Should().BeEquivalentTo(_fixture.Event);

    }
}
