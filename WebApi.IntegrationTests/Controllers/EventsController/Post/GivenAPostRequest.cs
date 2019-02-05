//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc.Testing;
//using WebApi.Models;
//using Xunit;

//namespace WebApi.IntegrationTests.Controllers.EventsController.Post
//{
//    public class GivenAPostRequest : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
//    {
//        private readonly WebApplicationFactory<Startup> _factory;
//        private HttpResponseMessage _response;
//        private HttpClient _client;

//        public GivenAPostRequest(WebApplicationFactory<Startup> factory)
//        {
//            _factory = factory;
//        }

//        public async Task InitializeAsync()
//        {
//            _client = _factory.CreateClient();
//            _response = await _client.GetAsync("/api/events");
//        }

//        [Fact]
//        public void ThenASuccessStatusCodeIsReturned()
//        {
//            _response.IsSuccessStatusCode.Should().BeTrue();
//        }

//        [Fact]
//        public void ThenAJsonContentTypeIsReturned()
//        {
//            _response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
//        }

//        [Fact]
//        public async Task ThenAllTestEventsAreReturned()
//        {
//            var events = await _response.Content.ReadAsAsync<ICollection<Event>>();

//            events.Count.Should().Be(3);
//        }

//        public Task DisposeAsync()
//        {
//            _client.Dispose();
//            return Task.CompletedTask;
//        }
//    }
//}
