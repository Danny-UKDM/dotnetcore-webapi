using System;
using System.Linq;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Queries;
using WebApi.Models.Events;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class GetTests
    {
        private readonly IQuerySession _session;
        private readonly EventsController _controller;

        public GetTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateQuerySession();
            _controller = new EventsController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoEvents()
        {
            _session
                .ExecuteAsync(Arg.Any<GetAllEventsQuery>())
                .Returns(Enumerable.Empty<Event>());

            var result = await _controller.GetAll();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEvents()
        {
            var (event1, _) = EventBuilder.CreateEvent("Cool Event").Build();
            var (event2, _) = EventBuilder.CreateEvent("Cooler Event").Build();
            var (event3, _) = EventBuilder.CreateEvent("Coolest Event").Build();

            _session
                .ExecuteAsync(Arg.Any<GetAllEventsQuery>())
                .Returns(new[]
                {
                    event1,
                    event2,
                    event3
                });

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(new[] {
                      event1,
                      event2,
                      event3
                  });
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoEventsForPartner()
        {
            _session
                .ExecuteAsync(Arg.Any<GetAllEventsQuery>())
                .Returns(Enumerable.Empty<Event>());

            var result = await _controller.GetAllForPartner(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEventsForPartner()
        {
            var partnerId = Guid.NewGuid();
            var (event1, _) = EventBuilder.CreateEvent("Cool Event").WithPartnerId(partnerId).Build();
            var (event2, _) = EventBuilder.CreateEvent("Cooler Event").WithPartnerId(partnerId).Build();
            var (event3, _) = EventBuilder.CreateEvent("Coolest Event").WithPartnerId(partnerId).Build();

            _session
                .ExecuteAsync(Arg.Is<GetEventsByPartnerIdQuery>(q => q.PartnerId == partnerId))
                .Returns(new[]
                {
                    event1,
                    event2,
                    event3
                });

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
        public async Task ReturnsOkWithEventWhenMatchingEventId()
        {
            var (@event, _) = EventBuilder.CreateEvent("Cool Event").Build();
            _session
                .ExecuteAsync(Arg.Is<GetEventByIdQuery>(q => q.EventId == @event.EventId))
                .Returns(@event);

            var result = await _controller.Get(@event.EventId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(@event);
        }
    }
}
