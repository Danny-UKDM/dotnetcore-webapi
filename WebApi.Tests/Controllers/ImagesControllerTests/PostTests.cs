using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Controllers.ImagesControllerTests
{
    public class PostTests
    {
        private readonly ImagesController _controller;
        private readonly IImageRepository _imageRepository;

        public PostTests()
        {
            _imageRepository = Substitute.For<IImageRepository>();
            _controller = new ImagesController(_imageRepository);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenFileIsNull()
        {
            var result = await _controller.Post(null);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsBadRequestWhenModelStateIsInvalid()
        {
            var file = new FormFile(Stream.Null, 0, 0, "Bad Test", "Bad Test");
            var createModelResult = new CreateModelResult<IFormFile>(ResultStatus.Failed, "Image is bad");

            _imageRepository
                .SaveImageAsync(file)
                .Returns(createModelResult);

            var result = await _controller.Post(file);

            result.Should().BeOfType<BadRequestObjectResult>().Which.Value
                  .Should().Be(createModelResult.Reason);
        }

        [Fact]
        public async Task ReturnsCreatedWhenModelStateValid()
        {
            var file = new FormFile(Stream.Null, 0,10, "Test", "Test");
            var createModelResult = new CreateModelResult<IFormFile>(file) { ImageId = Guid.NewGuid() };

            _imageRepository
                .SaveImageAsync(file)
                .Returns(createModelResult);

            var result = await _controller.Post(file);

            await _imageRepository.Received(1).SaveImageAsync(Arg.Is<FormFile>(c => c.Name == file.Name));

            result.Should().BeOfType<CreatedAtActionResult>().Which
                  .Should().BeEquivalentTo(new
                  {
                      ActionName = nameof(ImagesController.Get),
                      RouteValues = new RouteValueDictionary { { "imageId", createModelResult.ImageId } },
                      Value = file
                  }, o => o.ExcludingMissingMembers());
        }
    }
}
