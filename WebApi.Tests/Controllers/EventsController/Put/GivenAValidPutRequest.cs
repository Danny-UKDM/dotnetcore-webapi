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
            var builder = new EventBuilder();

            var @event = builder.CreateEvent("Updated Cool Event")
                                .InCity("New Cool City")
                                .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.UpdateEventAsync(@event).Returns(Task.CompletedTask);
            eventRepository.GetEventByIdAsync(@event.EventId).Returns(@event);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Put(@event.EventId, @event);
        }

        [Fact]
        public void ThenTheSatusCodeIs204NoContent()
        {
            _actionResult.Should().BeOfType<NoContentResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
