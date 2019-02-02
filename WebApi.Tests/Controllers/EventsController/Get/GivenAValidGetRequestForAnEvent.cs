using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Get
{
    public class GivenAValidGetRequestForAnEvent : IAsyncLifetime
    {
        private Event _event;
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var builder = new EventBuilder();

            _event = builder.CreateEvent("Cool Event")
                            .InCity("Cool City")
                            .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetEventByIdAsync(_event.EventId).Returns(_event);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.GetEventById(_event.EventId);
        }

        [Fact]
        public void ThenTheStatusCodeIs200Ok()
        {
            _actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenTheEventIsReturned()
        {
            var okObjectResult = _actionResult as OkObjectResult;
            var @event = okObjectResult.Value as Event;

            @event.Should().NotBeNull()
                  .And.BeEquivalentTo(_event);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
