using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using WebApi.IntegrationTests.Data;
using WebApi.IntegrationTests.Helpers;
using WebApi.Models;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Get
{
    public class GivenAGetRequestForAllEvents : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;

        public GivenAGetRequestForAllEvents(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ThenTheRequestSuccessfullyReturnsAllEvents()
        {
            await InsertTestEvents();

            var response = await _client.GetAsync("/api/events");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            var events = await response.Content.ReadAsAsync<ICollection<Event>>();
            events.Count.Should().Be(3);
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString($"Host=localhost;Username=postgres;Password=password;Pooling=false;Database=content")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }

        private async Task InsertTestEvents()
        {
            var event1 = new EventBuilder().CreateEvent("Cool Event").Build();
            var event2 = new EventBuilder().CreateEvent("Cooler Event").Build();
            var event3 = new EventBuilder().CreateEvent("Coolest Event").Build();

            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.CreateCommandSession())
            {
                await session.ExecuteAsync(new InsertEventCommand(event1));
                await session.ExecuteAsync(new InsertEventCommand(event2));
                await session.ExecuteAsync(new InsertEventCommand(event3));

                session.Commit();
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
