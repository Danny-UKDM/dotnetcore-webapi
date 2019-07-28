using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using FluentAssertions;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models.Events;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Get
{
    [Collection(nameof(TestCollection))]
    public class GivenAGetByPartnerRequest : IClassFixture<GivenAGetByPartnerRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Guid PartnerId { get; private set; }
            public IEnumerable<Event> Events { get; private set; }
            public HttpResponseMessage Response { get; private set; }
            public MemoryCache MemoryCache => MemoryCache.Default;

            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                PartnerId = Guid.NewGuid();
                var (event1, _) = EventBuilder.CreateEvent("Cool Event").WithPartnerId(PartnerId).Build();
                var (event2, _) = EventBuilder.CreateEvent("Cool Event").WithPartnerId(PartnerId).Build();
                Events = new List<Event> { event1, event2 };

                using (var session = _factory.SessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(event1));
                    await session.ExecuteAsync(new InsertEventCommand(event2));
                    session.Commit();
                }

                Response = await _factory.HttpClient
                                         .GetAsync($"/api/events/bypartner/{PartnerId}");
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

        public GivenAGetByPartnerRequest(GetRequest fixture) => _fixture = fixture;

        [Fact]
        public void ThenTheExpectedResponseWasReceived() =>
            _fixture.Response.StatusCode.Should().Be(HttpStatusCode.OK);

        [Fact]
        public void ThenTheEventsAreCached() =>
            _fixture.MemoryCache.Get(_fixture.PartnerId.ToString()).Should().BeEquivalentTo(_fixture.Events, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation)).WhenTypeIs<DateTime>());

        [Fact]
        public void ThenTheExpectedResponseContentTypeWasReceived() =>
            _fixture.Response.Content.Headers.ContentType.Should()
                    .BeEquivalentTo(new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

        [Fact]
        public async Task ThenTheResponseContentHasExpectedValue()
        {
            var events = await _fixture.Response.Content.ReadAsAsync<IEnumerable<Event>>();

            events.Should().BeEquivalentTo(_fixture.Events, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation, 500)).WhenTypeIs<DateTime>());
        }
    }
}
