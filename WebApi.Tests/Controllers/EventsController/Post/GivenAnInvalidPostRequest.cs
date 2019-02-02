using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Post
{
    public class GivenAnInvalidPostRequest : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var @event = new Event
            {
                Latitude = -8008,
                Longitude = 666
            };

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.AddEventAsync(@event).Returns(Task.CompletedTask);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            controller.ModelState.AddModelError(nameof(@event.Latitude), "Invalid Latitude.");
            controller.ModelState.AddModelError(nameof(@event.Longitude), "Invalid Longitude.");

            _actionResult = await controller.Post(@event);
        }

        [Fact]
        public void ThenTheStatusCodeIs400BadRequest()
        {
            _actionResult.Should().BeOfType<BadRequestResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
