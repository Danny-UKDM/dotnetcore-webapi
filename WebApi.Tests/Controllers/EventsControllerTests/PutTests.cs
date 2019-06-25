using System;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using WebApi.Models.Events;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class PutTests
    {
        private readonly ICommandSession _session;
        private readonly EventsController _controller;

        public PutTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateCommandSession();
            _controller = new EventsController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelErrors()
        {
            _controller.ModelState.AddModelError(nameof(Event.Latitude), "Invalid Latitude");
            _controller.ModelState.AddModelError(nameof(Event.Longitude), "Invalid Longitude");

            var result = await _controller.Put(Guid.NewGuid(), new Event());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenEventMissing()
        {
            _session
                .ExecuteAsync(Arg.Any<UpdateEventCommand>())
                .Returns(0);
            
            var result = await _controller.Put(Guid.NewGuid(), new Event());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var @event = EventBuilder.CreateEvent("Some Video").Build();
            _session
                .ExecuteAsync(Arg.Is<UpdateEventCommand>(c =>
                    c.EventId == @event.EventId &&
                    c.Details == @event))
                .Returns(1);

            var result = await _controller.Put(@event.EventId, @event);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Any<UpdateEventCommand>());
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
