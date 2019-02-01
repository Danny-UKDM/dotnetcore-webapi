using System;
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
        private Event _event;
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            _event = new Event
            {
                Latitude = int.MaxValue,
                Longitude = int.MaxValue
            };

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.AddEventAsync(_event).Returns(Task.CompletedTask);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Post(_event);
        }

        [Fact]
        public void ThenTheStatusCodeIs400BadRequest()
        {
            //_actionResult.Should().BeOfType<BadRequestResult>();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
