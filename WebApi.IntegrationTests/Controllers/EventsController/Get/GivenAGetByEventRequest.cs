using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models.Events;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Get
{
    [Collection(nameof(TestCollection))]
    public class GivenAGetByEventRequest : IClassFixture<GivenAGetByEventRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event Event { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Event = EventBuilder.CreateEvent("Cool Test Event")
                                    .InCity("Cool Test City")
                                    .Build();
                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(Event));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .GetAsync($"/api/events/byevent/{Event.EventId}");
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

        private readonly GetRequest _fixture;

        public GivenAGetByEventRequest(GetRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.OK);

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

        [Fact]
        public async Task ThenTheResponseContentHasExpectedValue()
        {
            var @event = await _fixture.Response.Content.ReadAsAsync<Event>();

            @event.Should().BeEquivalentTo(_fixture.Event, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation, 500)).WhenTypeIs<DateTime>());
        }
    }
}
