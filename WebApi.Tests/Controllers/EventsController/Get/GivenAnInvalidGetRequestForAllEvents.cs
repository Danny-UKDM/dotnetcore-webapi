using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Get
{
    public class GivenAnInvalidGetRequestForAllEvents : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetAllEventsAsync().Returns(new List<Event>());

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.GetAllEvents();
        }

        [Fact]
        public void ThenTheStatusCodeIs404NotFound()
        {
            _actionResult.Should().BeOfType<NotFoundResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
