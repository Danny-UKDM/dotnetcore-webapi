using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Writers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        //-- GET api/images/{imageId}
        [HttpGet("{imageId}")]
        [ProducesResponseType(200, Type = typeof(IFormFile))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid imageId)
        {
            return Ok(imageId);
            //return file != null
            //    ? Ok(file)
            //    : (IActionResult)NotFound();

        }

        //-- POST api/images
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(IFormFile))]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var modelResult = await _imageRepository.SaveImageAsync(file);

            if (modelResult.Result == ResultStatus.Failed)
                return BadRequest(modelResult);

            return CreatedAtAction(nameof(Get), new { imageId = modelResult.ImageId }, modelResult);
        }
    }
}
