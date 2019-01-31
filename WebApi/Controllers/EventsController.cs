using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Event))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            var events = await _eventRepository.GetAllEventsAsync();

            if (!events.Any())
                return NotFound();

            return Ok(events);
        }

        // GET api/events/{eventId}
        [HttpGet("{eventId}")]
        [ProducesResponseType(200, Type = typeof(Event))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int eventId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        // POST api/events
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Event))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] Event @event)
        {
            if (!TryValidateModel(@event))
            {
                return BadRequest();
            }

            await _eventRepository.AddEventAsync(@event);
            return StatusCode((int)HttpStatusCode.Created, @event);
        }

        //// PUT api/events/{eventId}
        //[HttpPut("{eventId}")]
        //public void Put(int eventId, [FromBody] string value)
        //{
        //}

        //// DELETE api/events/{eventId}
        //[HttpDelete("{eventId}")]
        //public void Delete(int eventId)
        //{
        //}
    }
}
