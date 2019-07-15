using System.Threading.Tasks;
using Badger.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data.Commands;
using WebApi.Models;
using WebApi.Models.Videos;
using WebApi.Tests.Helpers;
using Xunit;

namespace WebApi.Tests.Controllers.VideosControllerTests
{
    public class PostTests
    {
        private readonly ICommandSession _session;
        private readonly VideosController _controller;

        public PostTests()
        {
            var sessionFactory = Substitute.For<ISessionFactory>();
            _session = sessionFactory.CreateCommandSession();
            _controller = new VideosController(sessionFactory);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelErrors()
        {
            _controller.ModelState.AddModelError(nameof(Video.VideoName), "");

            var result = await _controller.Post(new Video());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsCreatedWhenModelStateValid()
        {
            var video = VideoBuilder.CreateVideo("Some Video").Build();

            var result = await _controller.Post(video);

            Received.InOrder(() =>
            {
                _session.Received(1).ExecuteAsync(Arg.Is<InsertVideoCommand>(c => c.Video == video));
                _session.Received(1).Commit();
                _session.Received(1).Dispose();
            });
            result.Should().BeOfType<CreatedAtActionResult>().Which
                  .Should().BeEquivalentTo(new
                  {
                      ActionName = nameof(VideosController.Get),
                      RouteValues = new RouteValueDictionary {{"videoId", video.VideoId}},
                      Value = video
                  });
        }
    }
}
