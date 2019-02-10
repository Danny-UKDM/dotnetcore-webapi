using System;
using System.Net.Http;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Get
{
    public class GivenAGetRequestForAnEvent : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private static ISessionFactory _sessionFactory;
        private static Event _event;

        public GivenAGetRequestForAnEvent(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _sessionFactory = DataUtils.CreateSessionFactory();

            _event = new EventBuilder().CreateEvent("Cool Test Event")
                                       .InCity("Cool Test City")
                                       .Build();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyReturnsTheEvent()
        {
            InsertTestEvent();

            var response = await _client.GetAsync($"/api/events/{_event.EventId}");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            var @event = await response.Content.ReadAsAsync<Event>();
            @event.Should().NotBeNull();
        }

        private static void InsertTestEvent()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new InsertEventCommand(_event));
                session.Commit();
            }
        }

        public void Dispose()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new DeleteRowsByEventIdCommand(new[] { _event.EventId }));
                session.Commit();
            }
        }
    }
}
