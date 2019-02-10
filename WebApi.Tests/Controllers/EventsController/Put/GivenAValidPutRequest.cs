using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Put
{
    public class GivenAValidPutRequest : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var existingEvent = new EventBuilder().CreateEvent("OG Cool Event")
                                                  .InCity("OG Cool City")
                                                  .Build();
            var newEvent = new EventBuilder().CreateEvent("Updated Cool Event")
                                           .InCity("New Cool City")
                                           .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.UpdateEventAsync(newEvent, existingEvent.EventId).Returns(Task.CompletedTask);
            eventRepository.GetEventByIdAsync(existingEvent.EventId).Returns(existingEvent);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Put(existingEvent.EventId, newEvent);
        }

        [Fact]
        public void ThenTheSatusCodeIs204NoContent()
        {
            _actionResult.Should().BeOfType<NoContentResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
