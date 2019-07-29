using System;
using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using WebApi.Models;
using WebApi.Models.Videos;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.VideosControllerTests
{
    public class PutTests
    {
        private readonly ICommandSession _session;
        private readonly VideosController _controller;

        public PutTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateCommandSession();
            _controller = new VideosController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelErrors()
        {
            _controller.ModelState.AddModelError(nameof(Video.VideoName), "");

            var result = await _controller.Put(Guid.NewGuid(), new Video());

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenVideoMissing()
        {
            _session
                .ExecuteAsync(Arg.Any<UpdateVideoCommand>())
                .Returns(0);
            
            var result = await _controller.Put(Guid.NewGuid(), new Video());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var video = VideoBuilder.CreateVideo("Some Video").Build();
            _session
                .ExecuteAsync(Arg.Is<UpdateVideoCommand>(c =>
                    c.VideoId == video.VideoId &&
                    c.Details == video))
                .Returns(1);

            var result = await _controller.Put(video.VideoId, video);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Any<UpdateVideoCommand>());
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
