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

namespace WebApi.IntegrationTests.Controllers.EventsController.Post
{
    public class GivenAPostRequest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly ISessionFactory _sessionFactory;
        private readonly Event _event;

        public GivenAPostRequest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _sessionFactory = DataUtils.CreateSessionFactory();

            _event = new EventBuilder().CreateEvent("Nice Test Event")
                                       .InCity("Nice Test City City")
                                       .Build();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyAddsTheEvent()
        {
            var eventJson = JsonConvert.SerializeObject(_event);
            var stringContent = new StringContent(eventJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/events", stringContent);

            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            using (var session = _sessionFactory.CreateQuerySession())
            {
                session.Execute(new CountRowsByEventIdQuery(new[] {_event.EventId})).Should().Be(1);
            }
        }

        public void Dispose()
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                session.Execute(new DeleteRowsByEventIdCommand(new[] {_event.EventId}));
                session.Commit();
            }
        }
    }
}
