using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class DeleteTests
    {
        private readonly EventsController _controller;
        private readonly IEventRepository _eventRepository;

        public DeleteTests()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _controller = new EventsController(_eventRepository);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenEventMissing()
        {
            _eventRepository
                .DeleteEventAsync(Arg.Any<Guid>())
                .Returns(false);

            var result = await _controller.Delete(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var eventId = Guid.NewGuid();

            _eventRepository
                .DeleteEventAsync(Arg.Is<Guid>(g => g == eventId))
                .Returns(true);

            var result = await _controller.Delete(eventId);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
