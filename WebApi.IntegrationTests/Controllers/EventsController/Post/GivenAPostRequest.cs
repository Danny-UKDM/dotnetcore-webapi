using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Post
{
    public class GivenAPostRequest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;

        public GivenAPostRequest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyAddsTheEvent()
        {
            var @event = new EventBuilder().CreateEvent("Cool Event")
                                           .InCity("Cool City")
                                           .Build();

            var eventJson = JsonConvert.SerializeObject(@event);
            var stringContent = new StringContent(eventJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/events", stringContent);

            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            var sessionFactory = DataUtils.CreateSessionFactory();
            using (var session = sessionFactory.CreateQuerySession())
            {
                session.Execute(new QueryForAllRowsCount()).Should().Be(1);
            }
        }

        public void Dispose()
        {
            DataUtils.PurgeTestData();
        }
    }
}
