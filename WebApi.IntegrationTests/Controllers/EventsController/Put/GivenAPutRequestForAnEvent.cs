using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Put
{
    public class GivenAPutRequestForAnEvent : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private static ISessionFactory _sessionFactory;
        private static Event _originalEvent;
        private readonly Event _updatedEvent;

        public GivenAPutRequestForAnEvent(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _sessionFactory = DataUtils.CreateSessionFactory();

            _originalEvent = new EventBuilder().CreateEvent("Cool OG Event")
                                               .InCity("Cool OG City")
                                               .Build();

            _updatedEvent = new EventBuilder().CreateEvent("Cool New Event")
                                              .InCity("Cool New City")
                                              .Build();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyUpdatesTheEvent()
        {
            InsertTestEvent();

            var eventJson = JsonConvert.SerializeObject(_updatedEvent);
            var httpContent = new StringContent(eventJson, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/events/{_originalEvent.EventId}", httpContent);
            response.IsSuccessStatusCode.Should().BeTrue();

            var newEvent = GetUpdatedEvent(_updatedEvent.EventId);
            newEvent.Should().BeEquivalentTo(_updatedEvent);
        }

        private static void InsertTestEvent()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new InsertEventCommand(_originalEvent));
                session.Commit();
            }
        }

        private static Event GetUpdatedEvent(Guid eventId)
        {
            Event @event;

            using (var session = _sessionFactory.CreateQuerySession())
            {
                @event = session.Execute(new GetEventByIdQuery(eventId));
            }

            return @event;
        }

        public void Dispose()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new DeleteRowsByEventIdCommand(new[] { _originalEvent.EventId, _updatedEvent.EventId }));
                session.Commit();
            }
        }
    }
}
