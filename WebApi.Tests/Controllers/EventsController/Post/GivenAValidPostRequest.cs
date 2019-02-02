using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Post
{
    public class GivenAValidPostRequest : IAsyncLifetime
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
            eventRepository.AddEventAsync(_event).Returns(Task.CompletedTask);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            _actionResult = await controller.Post(_event);
        }

        [Fact]
        public void ThenTheStatusCodeIs201Created()
        {
            _actionResult.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public void ThenTheEventIsAddedSuccessfully()
        {
            var createdResponse = _actionResult as CreatedAtActionResult;
            var @event = createdResponse.Value as Event;

            @event.Should().NotBeNull()
                  .And.BeEquivalentTo(_event);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
