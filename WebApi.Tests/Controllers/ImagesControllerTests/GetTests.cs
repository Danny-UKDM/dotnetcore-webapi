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
    public class GetTests
    {
        private readonly ImagesController _controller;
        private readonly Guid _imageId;
        private readonly ReadModelResult<byte[]> _readModelResult;

        public GetTests()
        {
            _imageId = Guid.NewGuid();
            var imageRepository = Substitute.For<IImageRepository>();
            imageRepository
                .GetImageAsync(Guid.Empty)
                .Returns(new ReadModelResult<byte[]>(ResultStatus.Failed));

            _readModelResult = new ReadModelResult<byte[]>(_imageId)
            {
                ContentType = "image/gif",
                Data = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x1, 0x0, 0x1, 0x0, 0x80, 0x0, 0x0, 0xff, 0xff, 0xff, 0x0, 0x0, 0x0, 0x2c, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x2, 0x2, 0x44, 0x1, 0x0, 0x3b }
            };
            imageRepository
                .GetImageAsync(_imageId)
                .Returns(_readModelResult);

            _controller = new ImagesController(imageRepository);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenIdIsMissing()
        {
            var result = await _controller.Get(Guid.Empty);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ReturnsOkWithImageWhenMatchingImageId()
        {
            var result = await _controller.Get(_imageId);

            result.Should().BeOfType<FileContentResult>().Which.FileContents
                  .Should().BeEquivalentTo(_readModelResult.Data);
        }
    }
}
