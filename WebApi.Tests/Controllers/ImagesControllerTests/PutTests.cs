using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Images;
using Xunit;

namespace WebApi.Tests.Controllers.ImagesControllerTests
{
    public class PutTests
    {
        private readonly IImageRepository _imageRepository;
        private readonly ImagesController _controller;


        public PutTests()
        {
            _imageRepository = Substitute.For<IImageRepository>();
            _controller = new ImagesController(_imageRepository);
        }

        [Fact]
        public async Task ReturnsBadRequestWhenIdOrFileIsNull()
        {
            var result = await _controller.Put(Guid.Empty, null);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReturnsNotFoundWhenUpdateFails()
        {
            var file = new FormFile(Stream.Null, 0, 0, "Bad Test", "Bad Test");
            var createModelResult = new CreateModelResult<IFormFile>(ResultStatus.Failed, "Update Failed");

            _imageRepository
                .SaveImageAsync(Arg.Any<FormFile>(), Arg.Any<Guid>())
                .Returns(createModelResult);

            var result = await _controller.Put(Guid.NewGuid(), file);

            result.Should().BeOfType<NotFoundObjectResult>().Which.Value
                  .Should().Be(createModelResult.Reason);
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var imageId = Guid.NewGuid();
            var file = new FormFile(Stream.Null, 0, 10, "Test", "Test");
            var createModelResult = new CreateModelResult<IFormFile>(file) { ImageId = imageId };

            _imageRepository
                .SaveImageAsync(Arg.Is<FormFile>(c =>
                    c.Name == file.Name &&
                    c.FileName == file.FileName), 
                    Arg.Is(imageId))
                .Returns(createModelResult);

            var result = await _controller.Put(imageId, file);

            await _imageRepository.Received(1).SaveImageAsync(Arg.Any<FormFile>(), Arg.Any<Guid>());
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
