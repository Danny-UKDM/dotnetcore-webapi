using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Delete
{
    public class GivenAValidDeleteRequest : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var builder = new EventBuilder();

            var @event = builder.CreateEvent("Cool Event")
                                .InCity("Cool City")
                                .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.DeleteEventAsync(@event.EventId).Returns(Task.CompletedTask);
            eventRepository.GetEventByIdAsync(@event.EventId).Returns(@event);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Delete(@event.EventId);
        }

        [Fact]
        public void ThenTheSatusCodeIs204NoContent()
        {
            _actionResult.Should().BeOfType<NoContentResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
