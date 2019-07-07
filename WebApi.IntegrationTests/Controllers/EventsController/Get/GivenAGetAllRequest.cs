using System;
using System.Collections.Generic;
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
    public class GivenAGetAllRequest : IClassFixture<GivenAGetAllRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event Event1 { get; private set; }
            public Event Event2 { get; private set; }
            public Event Event3 { get; private set; }
            public HttpResponseMessage Response { get; private set; }

            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                Event1 = EventBuilder.CreateEvent("Cool Test Event")
                                    .InCity("Cool Test City")
                                    .Build();
                Event2 = EventBuilder.CreateEvent("Cooler Test Event")
                                     .InCity("Cooler Test City")
                                     .Build();
                Event3 = EventBuilder.CreateEvent("Coolest Test Event")
                                     .InCity("Coolest Test City")
                                     .Build();

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(Event1));
                    await session.ExecuteAsync(new InsertEventCommand(Event2));
                    await session.ExecuteAsync(new InsertEventCommand(Event3));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .GetAsync("/api/events/all");
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

        public GivenAGetAllRequest(GetRequest fixture) => _fixture = fixture;

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
            var events = await _fixture.Response.Content.ReadAsAsync<IEnumerable<Event>>();

            events.Should().BeEquivalentTo(new[]
            {
                _fixture.Event1,
                _fixture.Event2,
                _fixture.Event3
            }, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation, 500)).WhenTypeIs<DateTime>());
        }
    }
}
