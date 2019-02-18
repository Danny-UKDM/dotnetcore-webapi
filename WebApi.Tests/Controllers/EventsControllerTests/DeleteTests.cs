using System;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using Xunit;

namespace WebApi.Tests.Controllers.EventsControllerTests
{
    public class DeleteTests
    {
        private readonly ICommandSession _session;
        private readonly EventsController _controller;

        public DeleteTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateCommandSession();
            _controller = new EventsController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenEventMissing()
        {
            _session
                .ExecuteAsync(Arg.Any<DeleteEventCommand>())
                .Returns(0);
            
            var result = await _controller.Delete(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var eventId = Guid.NewGuid();
            _session
                .ExecuteAsync(Arg.Is<DeleteEventCommand>(c =>
                    c.EventId == eventId))
                .Returns(1);

            var result = await _controller.Delete(eventId);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Any<DeleteEventCommand>());
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
