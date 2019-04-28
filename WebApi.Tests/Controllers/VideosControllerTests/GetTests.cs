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

namespace WebApi.Tests.Controllers.VideosControllerTests
{
    public class GetTests
    {
        private readonly IQuerySession _session;
        private readonly VideosController _controller;

        public GetTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateQuerySession();
            _controller = new VideosController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenNoVideos()
        {
            _session
                .ExecuteAsync(Arg.Any<GetAllVideosQuery>())
                .Returns(Enumerable.Empty<Video>());

            var result = await _controller.Get();

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsOkWithEventsWhenSomeEvents()
        {
            var one = VideoBuilder.CreateVideo("Cool Video").Build();
            var two = VideoBuilder.CreateVideo("Cooler Video").Build();
            var three = VideoBuilder.CreateVideo("Coolest Video").Build();
            _session
                .ExecuteAsync(Arg.Any<GetAllVideosQuery>())
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
        public async Task ReturnsNotFoundWhenNoMatchingVideoId()
        {
            var result = await _controller.Get(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ReturnsOkWithVideoWhenMatchingVideoId()
        {
            var video = VideoBuilder.CreateVideo("Some Video").Build();
            _session
                .ExecuteAsync(Arg.Is<GetVideoByIdQuery>(q => q.VideoId == video.VideoId))
                .Returns(video);

            var result = await _controller.Get(video.VideoId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(video);
        }
    }
}
