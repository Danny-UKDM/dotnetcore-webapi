﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Images;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository) =>
            _imageRepository = imageRepository;

        //-- GET api/images/{imageId}
        [HttpGet("{imageId}")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        [ProducesResponseType(404, Type = typeof(string))]
        public IActionResult Get(Guid imageId)
        {
            var modelResult = _imageRepository.GetImageAsync(imageId);

            return modelResult.Result != ResultStatus.Failed
                ? Ok(modelResult.Locations)
                : (IActionResult)NotFound(modelResult.Reason);
        }

        //-- POST api/images
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null)
                return BadRequest("File is null");

            var modelResult = await _imageRepository.SaveImageAsync(file);

            return modelResult.Result != ResultStatus.Failed
                ? CreatedAtAction(nameof(Get), new { imageId = modelResult.ImageId }, modelResult)
                : (IActionResult)BadRequest(modelResult.Reason);
        }

        //-- PUT api/images/{imageId}
        [HttpPut("{imageId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> Put(Guid imageId, IFormFile file)
        {
            if (imageId == Guid.Empty || file == null)
                return BadRequest("Bad imageId or file");

            var modelResult = await _imageRepository.SaveImageAsync(file, imageId);

            return modelResult.Result != ResultStatus.Failed
                ? NoContent()
                : (IActionResult)NotFound(modelResult.Reason);
        }

        //-- DELETE api/images/{imageId}
        [HttpDelete("{imageId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(string))]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var modelResult = await _imageRepository.DeleteImageAsync(imageId);

            return modelResult.Result != ResultStatus.Failed
                ? NoContent()
                : (IActionResult)NotFound(modelResult.Reason);
        }
    }
}
