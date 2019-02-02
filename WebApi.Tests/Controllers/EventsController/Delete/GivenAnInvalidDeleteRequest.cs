using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Delete
{
    public class GivenAnInvalidDeleteRequest : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var builder = new EventBuilder();

            var @event = builder.CreateEvent("Unknown Event")
                                .InCity("Alien City")
                                .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetEventByIdAsync(@event.EventId).Returns((Event)null);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Delete(@event.EventId);
        }

        [Fact]
        public void ThenTheSatusCodeIs404NotFound()
        {
            _actionResult.Should().BeOfType<NotFoundResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
