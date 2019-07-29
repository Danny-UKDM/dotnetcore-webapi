using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models.Events;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class PostTests
    {
        private readonly EventsController _controller;
        private readonly IEventRepository _eventRepository;

        public PostTests()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _controller = new EventsController(_eventRepository);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelErrors()
        {
            _controller.ModelState.AddModelError(nameof(EventWriteModel.Latitude), "Invalid Latitude");
            _controller.ModelState.AddModelError(nameof(EventWriteModel.Longitude), "Invalid Longitude");

            var result = await _controller.Post(new EventWriteModel());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsCreatedWhenModelStateValid()
        {
            var (@event, eventWriteModel) = EventBuilder.CreateEvent("Cool Event").Build();

            _eventRepository
                .SaveEventAsync(Arg.Is<EventWriteModel>(e => e == eventWriteModel))
                .Returns(@event);

            var result = await _controller.Post(eventWriteModel);

            result.Should().BeOfType<CreatedAtActionResult>().Which
                  .Should().BeEquivalentTo(new
                  {
                      ActionName = nameof(EventsController.Get),
                      RouteValues = new RouteValueDictionary { { "eventId", @event.EventId } },
                      Value = @event
                  });
        }
    }
}
