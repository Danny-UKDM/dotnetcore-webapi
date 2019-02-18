using System;
using System.Linq;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Queries;
using WebApi.Models;
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

            var result = await _controller.Get();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEvents()
        {
            var one = EventBuilder.CreateEvent("Cool Event")
                                  .InCity("Cool City")
                                  .Build();
            var two = EventBuilder.CreateEvent("Cooler Event")
                                  .InCity("Cooler City")
                                  .Build();
            var three = EventBuilder.CreateEvent("Coolest Event")
                                    .InCity("Coolest City")
                                    .Build();
            _session
                .ExecuteAsync(Arg.Any<GetAllEventsQuery>())
                .Returns(new[]
                {
                    one,
                    two,
                    three
                });

            var result = await _controller.Get();

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(new [] {
                      one,
                      two,
                      three
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
            var @event = EventBuilder.CreateEvent("Some Event").Build();
            _session
                .ExecuteAsync(Arg.Is<GetEventByIdQuery>(q => q.EventId == @event.EventId))
                .Returns(@event);

            var result = await _controller.Get(@event.EventId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(@event);
        }
    }
}
