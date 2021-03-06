﻿using System;
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
    public class GivenAGetByEventRequest : IClassFixture<GivenAGetByEventRequest.GetRequest>
    {
        public class GetRequest : IAsyncLifetime
        {
            private readonly ApiWebApplicationFactory _factory;
            public Event Event { get; private set; }
            public HttpResponseMessage Response { get; private set; }
            public MemoryCache MemoryCache => MemoryCache.Default;

            public GetRequest(ApiWebApplicationFactory factory) => _factory = factory;

            public async Task InitializeAsync()
            {
                var (@event, _) = EventBuilder.CreateEvent("Cool Event").Build();
                Event = @event;

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
        public void ThenTheEventsAreCached() =>
            _fixture.MemoryCache.Get(_fixture.Event.EventId.ToString()).Should().BeEquivalentTo(_fixture.Event, o =>
                o.Using<DateTime>(t => t.Subject.Should()
                                        .BeCloseTo(t.Expectation)).WhenTypeIs<DateTime>());

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
