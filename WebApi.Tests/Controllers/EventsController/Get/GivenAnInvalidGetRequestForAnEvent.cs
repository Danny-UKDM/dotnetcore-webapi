using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Get
{
    public class GivenAnInvalidGetRequestForAnEvent : IAsyncLifetime
    {
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetEventByIdAsync(Arg.Any<Guid>()).Returns(new Event());

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Get();
        }

        [Fact]
        public void ThenTheStatusCodeIs404NotFound()
        {
            _actionResult.Should().BeOfType<NotFoundResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
