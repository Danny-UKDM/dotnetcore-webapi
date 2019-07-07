using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models.Events;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Delete
{
    [Collection(nameof(TestCollection))]
    public class GivenADeleteRequest : IClassFixture<GivenADeleteRequest.DeleteRequest>
    {
        public class DeleteRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event Event { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public DeleteRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Event = EventBuilder.CreateEvent("Cool Removable Event")
                                    .InCity("Cool Removable City")
                                    .Build();
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(Event));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .DeleteAsync($"/api/events/{Event.EventId}");
            }

            public async Task<IEnumerable<Event>> LoadStoredEvents()
            {
                using (var session = _factory.SessionFactory.CreateQuerySession())
                {
                    return await session.ExecuteAsync(new GetEventsQuery());
                }
            }

            public async Task DisposeAsync()
            {
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteEventsCommand());
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
        public async Task ThenTheEventShouldNotExist() =>
            (await _fixture.LoadStoredEvents()).Should().BeNullOrEmpty();
    }
}
