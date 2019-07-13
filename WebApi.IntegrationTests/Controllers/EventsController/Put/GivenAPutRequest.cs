using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models.Events;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Put
{
    [Collection(nameof(TestCollection))]
    public class GivenAPutRequest : IClassFixture<GivenAPutRequest.PutRequest>
    {
        public class PutRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event UpdatedEvent { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public PutRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                var partnerId = Guid.NewGuid();
                var (originalEvent, _) = EventBuilder.CreateEvent("Cool OG Event")
                                                       .WithPartnerId(partnerId)
                                                       .Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    session.Execute(new InsertEventCommand(originalEvent));
                    session.Commit();
                }

                var (updatedEvent, updatedEventWriteModel) = EventBuilder.CreateEvent("Cool Updated Event")
                                                                           .WithPartnerId(partnerId)
                                                                           .WithEventId(originalEvent.EventId)
                                                                           .Build();
                UpdatedEvent = updatedEvent;

                var httpContent = new ObjectContent<EventWriteModel>(updatedEventWriteModel, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.HttpClient
                                         .PutAsync($"/api/events/{originalEvent.EventId}", httpContent);
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

        private readonly PutRequest _fixture;

        public GivenAPutRequest(PutRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        [Fact]
        public async Task ThenTheEventShouldBeUpdated()
        {
            var storedEvent = (await _fixture.LoadStoredEvents()).First();

            storedEvent.Should().BeEquivalentTo(_fixture.UpdatedEvent, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation, 500)).WhenTypeIs<DateTime>());
        }
    }
}
