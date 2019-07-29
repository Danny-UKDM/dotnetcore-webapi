using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badger.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApi.Data.Commands;
using WebApi.Data.Queries;
using WebApi.Models;
using WebApi.Models.Videos;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly ISessionFactory _sessionFactory;

        public VideosController(ISessionFactory sessionFactory) =>
            _sessionFactory = sessionFactory;

        //-- GET api/videos
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Video>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var videos = await session.ExecuteAsync(new GetAllVideosQuery());
                return videos.Any()
                    ? Ok(videos)
                    : (IActionResult)NotFound();
            }
        }

        //-- GET api/videos/{videoId}
        [HttpGet("{videoId}")]
        [ProducesResponseType(200, Type = typeof(Video))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid videoId)
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var video = await session.ExecuteAsync(new GetVideoByIdQuery(videoId));

                return video != null
                    ? Ok(video)
                    : (IActionResult)NotFound();
            }
        }

        //-- POST api/videos
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(Video))]
        public async Task<IActionResult> Post([BindRequired, FromBody]Video video)
        {
            if (!ModelState.IsValid)
                return BadRequest(video);

            using (var session = _sessionFactory.CreateCommandSession())
            {
                await session.ExecuteAsync(new InsertVideoCommand(video));
                session.Commit();
            }

            return CreatedAtAction(nameof(Get), new { videoId = video.VideoId }, video);
        }

        //-- PUT api/videos/{videoId}
        [HttpPut("{videoId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(Guid videoId, [BindRequired, FromBody]Video video)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new UpdateVideoCommand(videoId, video));
                session.Commit();

                return affected != 0
                    ? NoContent()
                    : (IActionResult)NotFound();
            }
        }

        //-- DELETE api/videos/{videoId}
        [HttpDelete("{videoId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid videoId)
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new DeleteVideoCommand(videoId));
                session.Commit();

                return affected != 0
                    ? NoContent()
                    : (IActionResult)NotFound();
            }
        }
    }
}
