using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Models;
using WebApi.Services;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsController.Put
{
    public class GivenAnInvalidPutRequest
    {
        private Event _event;
        private WebApi.Controllers.EventsController _controller;

        public GivenAnInvalidPutRequest()
        {
            var builder = new EventBuilder();
            _event = builder.CreateEvent("Uncool Event").Build();

            var eventRepository = Substitute.For<IEventRepository>();
            eventRepository.GetEventByIdAsync(_event.EventId).Returns((Event)null);

            _controller = new WebApi.Controllers.EventsController(eventRepository);
        }

        [Fact]
        public async Task WhenTheModelStateIsInvalid_ThenTheStatusCodeIs400BadRequestAsync()
        {
            _event.Latitude = 666;
            _event.Longitude = 8008;
            _controller.ModelState.AddModelError(nameof(_event.Latitude), "Invalid Latitude.");
            _controller.ModelState.AddModelError(nameof(_event.Longitude), "Invalid Longitude.");

            var actionResult = await _controller.Put(_event.EventId, _event);
            actionResult.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task WhenNoExistingEvent_ThenTheStatusCodeIs404NotFoundAsync()
        {
            var actionResult = await _controller.Put(_event.EventId, _event);
            actionResult.Should().BeOfType<NotFoundResult>();
        }
    }
}
