using System.Collections.Generic;
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
            public HttpResponseMessage Response { get; private set; }

            public DeleteRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                var (@event, _) = EventBuilder.CreateEvent("Cool Removable Event").Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(@event));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .DeleteAsync($"/api/events/{@event.EventId}");
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
