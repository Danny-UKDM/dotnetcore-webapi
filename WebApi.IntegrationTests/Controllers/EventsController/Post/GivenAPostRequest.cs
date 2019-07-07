using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models.Events;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Post
{
    [Collection(nameof(TestCollection))]
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
                                    .InCity("Nice Test City")
                                    .Build();

                var eventWriteModel = Event.ToEventWriteModel();

                var httpContent = new ObjectContent<EventWriteModel>(eventWriteModel, new JsonMediaTypeFormatter(), "application/json");

                Response = await _factory.HttpClient
                                         .PostAsync("/api/events", httpContent);
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

        private readonly PostRequest _fixture;

        public GivenAPostRequest(PostRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseStatusCodeWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.Created);

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

        [Fact]
        public async Task ThenTheEventShouldBeStored()
        {
            var storedEvent = (await _fixture.LoadStoredEvents()).First();

            storedEvent.Should().BeEquivalentTo(_fixture.Event, o =>
                o.Excluding(e => e.EventId)
                 .Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation, 500)).WhenTypeIs<DateTime>());

            storedEvent.EventId.Should().NotBeEmpty()
                       .And.Subject.HasValue.Should().BeTrue();
        }
    }
}
