using System.Collections.Generic;
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
    public class GivenAValidGetRequestForAllEvents : IAsyncLifetime
    {
        private Event _event1;
        private Event _event2;
        private Event _event3;
        private IActionResult _actionResult;

        public async Task InitializeAsync()
        {
            var builder = new EventBuilder();

            _event1 = builder.CreateEvent("Cool Event")
                                .InCity("Cool City")
                                .Build();

            _event2 = builder.CreateEvent("Cooler Event")
                                .InCity("Cooler City")
                                .Build();

            _event3 = builder.CreateEvent("Coolest Event")
                                .InCity("Coolest City")
                                .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetAllEventsAsync().Returns(new List<Event> { _event1, _event2, _event3 });

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.GetAllEvents();
        }

        [Fact]
        public void ThenTheStatusCodeIs200Ok()
        {
            _actionResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenAllEventsAreReturned()
        {
            var okObjectResult = _actionResult as OkObjectResult;
            var events = okObjectResult.Value as IEnumerable<Event>;

            events.Should().NotBeEmpty()
                  .And.HaveCount(3)
                  .And.ContainInOrder(_event1, _event2, _event3);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
