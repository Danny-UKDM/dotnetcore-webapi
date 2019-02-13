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

namespace WebApi.IntegrationTests.Controllers.EventsController.Delete
{
    public class GivenADeleteRequestForAnEvent : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private static ISessionFactory _sessionFactory;
        private static Event _event;

        public GivenADeleteRequestForAnEvent(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _sessionFactory = DataUtils.CreateSessionFactory();

            _event = new EventBuilder().CreateEvent("Cool Removable Event")
                                       .InCity("Cool Removable City")
                                       .Build();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyDeletesTheEvent()
        {
            InsertTestEvent();

            var response = await _client.DeleteAsync($"/api/events/{_event.EventId}");
            response.IsSuccessStatusCode.Should().BeTrue();

            EventExists(_event.EventId).Should().BeFalse();
        }

        private static void InsertTestEvent()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new InsertEventCommand(_event));
                session.Commit();
            }
        }

        private static bool EventExists(Guid eventId)
        {
            long count;

            using (var session = _sessionFactory.CreateQuerySession())
            {
                count = session.Execute(new CountRowsByEventIdQuery(new[] { eventId }));
            }

            return count > 0 ? true : false;
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
