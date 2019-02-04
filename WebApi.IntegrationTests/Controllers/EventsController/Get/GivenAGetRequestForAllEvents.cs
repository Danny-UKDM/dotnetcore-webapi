using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController.Get
{
    public class GivenAGetRequestForAllEvents : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private HttpResponseMessage _response;
        private HttpClient _client;

        public GivenAGetRequestForAllEvents(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();
            _response = await _client.GetAsync("/api/events");
        }

        [Fact]
        public void ThenASuccessStatusCodeIsReturned()
        {
            _response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public void ThenAJsonContentTypeIsReturned()
        {
            _response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public void ThenTheContentLengthIsNotZero()
        {
            _response.Content.Headers.ContentLength.Should().BeGreaterThan(0);
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }
    }
}
