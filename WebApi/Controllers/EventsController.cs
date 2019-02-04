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
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        //-- GET api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();

            if (!events.Any())
                return NotFound();

            return Ok(events);
        }

        //-- GET api/events/{eventId}
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);

            if (@event == null)
                return NotFound();

            return Ok(@event);
        }

        //-- POST api/events
        [HttpPost]
        public async Task<IActionResult> Post([BindRequired, FromBody]Event @event)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _eventRepository.AddEventAsync(@event);
            return CreatedAtAction(nameof(GetEventById), new { eventId = @event.EventId }, @event);
        }

        //-- PUT api/events/{eventId}
        [HttpPut("{eventId}")]
        public async Task<IActionResult> Put(Guid eventId, [BindRequired, FromBody]Event @event)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var existingEvent = await _eventRepository.GetEventByIdAsync(eventId);

            if (existingEvent == null)
                return NotFound();

            await _eventRepository.UpdateEventAsync(@event);
            return NoContent();
        }

        //-- DELETE api/events/{eventId}
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            var existingEvent = await _eventRepository.GetEventByIdAsync(eventId);

            if (existingEvent == null)
                return NotFound();

            await _eventRepository.DeleteEventAsync(eventId);
            return NoContent();
        }
    }
}
