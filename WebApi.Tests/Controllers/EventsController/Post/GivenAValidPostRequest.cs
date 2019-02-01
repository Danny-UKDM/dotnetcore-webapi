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
        private ObjectResult _createdResponse;

        public async Task InitializeAsync()
        {
            var builder = new EventBuilder();

            _event = builder.CreateEvent("Cool Event")
                                .InCity("Cool City")
                                .Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.AddEventAsync(_event).Returns(Task.CompletedTask);

            var controller = new WebApi.Controllers.EventsController(eventRepository);
            var actionResult = await controller.Post(_event);
            _createdResponse = actionResult as ObjectResult;
        }

        [Fact]
        public void ThenTheStatusCodeIs201Created()
        {
            _createdResponse.StatusCode.Should().Be(201);
        }

        [Fact]
        public void ThenTheEventIsAddedSuccessfully()
        {
            var @event = _createdResponse.Value as Event;

            @event.Should().NotBeNull()
                  .And.BeEquivalentTo(_event);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
