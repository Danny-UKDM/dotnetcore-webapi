using System;
using System.Linq;
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
    public class GetTests
    {
        private readonly EventsController _controller;
        private readonly IEventRepository _eventRepository;

        public GetTests()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _controller = new EventsController(_eventRepository);
        }

        [Fact]
        public async Task ReturnsOkWithEventWhenMatchingEventId()
        {
            var (@event, _) = EventBuilder.CreateEvent("Cool Event").Build();

            _eventRepository
                .GetEventByIdAsync(Arg.Is<Guid>(g => g == @event.EventId))
                .Returns(@event);

            var result = await _controller.Get(@event.EventId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(@event);
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEvents()
        {
            var (event1, _) = EventBuilder.CreateEvent("Cool Event").Build();
            var (event2, _) = EventBuilder.CreateEvent("Cooler Event").Build();
            var (event3, _) = EventBuilder.CreateEvent("Coolest Event").Build();

            _eventRepository
                .GetAllEventsAsync()
                .Returns(new[] { event1, event2, event3 });

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(new[] {
                      event1,
                      event2,
                      event3
                  });
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEventsForPartner()
        {
            var partnerId = Guid.NewGuid();
            var (event1, _) = EventBuilder.CreateEvent("Cool Event").WithPartnerId(partnerId).Build();
            var (event2, _) = EventBuilder.CreateEvent("Cooler Event").WithPartnerId(partnerId).Build();
            var (event3, _) = EventBuilder.CreateEvent("Coolest Event").WithPartnerId(partnerId).Build();

            _eventRepository
                .GetAllEventsForPartnerAsync(Arg.Is<Guid>(g => g == partnerId))
                .Returns(new[] { event1, event2, event3 });

            var result = await _controller.GetAllForPartner(partnerId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(new[] {
                      event1,
                      event2,
                      event3
                  });
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoMatchingEventId()
        {
            var result = await _controller.Get(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoEvents()
        {
            _eventRepository
                .GetAllEventsAsync()
                .Returns(Enumerable.Empty<Event>());

            var result = await _controller.GetAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoEventsForPartner()
        {
            _eventRepository
                .GetAllEventsForPartnerAsync(Arg.Any<Guid>())
                .Returns(Enumerable.Empty<Event>());

            var result = await _controller.GetAllForPartner(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
