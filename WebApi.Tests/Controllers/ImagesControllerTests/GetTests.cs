using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Controllers.ImagesControllerTests
{
    public class GetTests
    {
        private readonly ImagesController _controller;
        private readonly Guid _imageId;
        private readonly ReadModelResult _readModelResult;

        public GetTests()
        {
            _imageId = Guid.NewGuid();
            _readModelResult = new ReadModelResult(_imageId)
            {
                Locations = new[] { "https://someimagelocation.com" }
            };

            var imageRepository = Substitute.For<IImageRepository>();
            imageRepository
                .GetImageAsync(Guid.Empty)
                .Returns(new ReadModelResult(ResultStatus.Failed));
            imageRepository
                .GetImageAsync(_imageId)
                .Returns(_readModelResult);

            _controller = new ImagesController(imageRepository);
        }

        [Fact]
        public void ReturnsNotFoundWhenIdIsMissing()
        {
            var result = _controller.Get(Guid.Empty);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void ReturnsOkWithImageWhenMatchingImageId()
        {
            var result = _controller.Get(_imageId);

            result.Should().BeOfType<OkObjectResult>().Which.Value
                  .Should().BeEquivalentTo(_readModelResult.Locations);
        }
    }
}
