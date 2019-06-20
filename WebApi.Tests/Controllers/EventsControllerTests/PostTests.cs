using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using WebApi.Models;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class PostTests
    {
        private readonly ICommandSession _session;
        private readonly EventsController _controller;

        public PostTests()
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

            var result = await _controller.Post(new Event());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsCreatedWhenModelStateValid()
        {
            var @event = EventBuilder.CreateEvent("Some Event").Build();

            var result = await _controller.Post(@event);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Is<InsertEventCommand>(c => c.Event == @event));
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<CreatedAtActionResult>().Which
                  .Should().BeEquivalentTo(new
                  {
                      ActionName = nameof(EventsController.Get),
                      RouteValues = new RouteValueDictionary {{"eventId", @event.EventId}},
                      Value = @event
                  });
        }
    }
}
