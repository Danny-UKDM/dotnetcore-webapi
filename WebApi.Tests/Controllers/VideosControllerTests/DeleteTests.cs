using System;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using Xunit;

namespace WebApi.Tests.Controllers.VideosControllerTests
{
    public class DeleteTests
    {
        private readonly ICommandSession _session;
        private readonly VideosController _controller;

        public DeleteTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateCommandSession();
            _controller = new VideosController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenVideoMissing()
        {
            _session
                .ExecuteAsync(Arg.Any<DeleteVideoCommand>())
                .Returns(0);
            
            var result = await _controller.Delete(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var videoId = Guid.NewGuid();
            _session
                .ExecuteAsync(Arg.Is<DeleteVideoCommand>(c =>
                    c.VideoId == videoId))
                .Returns(1);

            var result = await _controller.Delete(videoId);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Any<DeleteVideoCommand>());
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
