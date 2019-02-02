using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<IActionResult> GetAllEvents()
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
        public async Task<IActionResult> GetEventById(Guid eventId)
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
        public async Task<IActionResult> Post([BindRequired, FromBody] Event @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _eventRepository.AddEventAsync(@event);
            return CreatedAtAction(nameof(GetEventById), new { eventId = @event.EventId }, @event);
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
