using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models.Events;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class PutTests
    {
        private readonly EventsController _controller;
        private readonly IEventRepository _eventRepository;

        public PutTests()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _controller = new EventsController(_eventRepository);
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var (@event, eventWriteModel) = EventBuilder.CreateEvent("Cool Event").Build();

            _eventRepository
                .UpdateEventAsync(Arg.Is<Guid>(g => g == @event.EventId),
                    Arg.Is<EventWriteModel>(e => e == eventWriteModel))
                .Returns(true);

            var result = await _controller.Put(@event.EventId, eventWriteModel);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelErrors()
        {
            _controller.ModelState.AddModelError(nameof(Event.Latitude), "Invalid Latitude");
            _controller.ModelState.AddModelError(nameof(Event.Longitude), "Invalid Longitude");

            var result = await _controller.Put(Guid.NewGuid(), new EventWriteModel());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenEventMissing()
        {
            _eventRepository
                .UpdateEventAsync(Arg.Any<Guid>(), Arg.Any<EventWriteModel>())
                .Returns(false);

            var result = await _controller.Put(Guid.NewGuid(), new EventWriteModel());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
