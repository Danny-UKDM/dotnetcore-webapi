using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Put
{
    [Collection(nameof(TestCollection))]
    public class GivenAPutRequest : IClassFixture<GivenAPutRequest.PutRequest>
    {
        public class PutRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            private Event _originalEvent;
            public Event UpdatedEvent { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public PutRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                _originalEvent = EventBuilder.CreateEvent("Cool OG Event")
                                             .InCity("Cool OG City")
                                             .Build();
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    session.Execute(new InsertEventCommand(_originalEvent));
                    session.Commit();
                }

                UpdatedEvent = EventBuilder.CreateEvent("Cool New Event")
                                           .InCity("Cool New City")
                                           .WithId(_originalEvent.EventId)
                                           .Build();

                var httpContent = new ObjectContent<Event>(UpdatedEvent, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.HttpClient
                                         .PutAsync($"/api/events/{_originalEvent.EventId}", httpContent);
            }

            public async Task<Event> LoadStoredEvent()
            {
                using (var session = _factory.SessionFactory.CreateQuerySession())
                {
                    return await session.ExecuteAsync(new GetEventByIdQuery(UpdatedEvent.EventId));
                }
            }

            public async Task DisposeAsync()
            {
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteRowsByEventIdCommand(new[] { _originalEvent.EventId, UpdatedEvent.EventId }));
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
        public async Task ThenTheEventShouldBeUpdated() =>
            (await _fixture.LoadStoredEvent()).Should().BeEquivalentTo(_fixture.UpdatedEvent);
    }
}
