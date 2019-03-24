using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Controllers.ImagesControllerTests
{
    public class DeleteTests
    {
        private readonly ImagesController _controller;
        private readonly Guid _imageId;

        public DeleteTests()
        {
            var imageRepository = Substitute.For<IImageRepository>();
            imageRepository
                .DeleteImageAsync(Guid.Empty)
                .Returns(new DeleteModelResult(ResultStatus.Failed));

            _imageId = Guid.NewGuid();
            imageRepository
                .DeleteImageAsync(_imageId)
                .Returns(new DeleteModelResult(_imageId));

            _controller = new ImagesController(imageRepository);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenIdIsMissing()
        {
            var result = await _controller.Delete(Guid.Empty);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ReturnsNoContentWhenUpdateSuccessful()
        {
            var result = await _controller.Delete(_imageId);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
