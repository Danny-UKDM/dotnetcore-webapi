using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApi.Data;
using WebApi.Models.Events;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository) =>
            _eventRepository = eventRepository;

        //-- GET api/Events/All
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Event>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return events.Any()
                ? Ok(events)
                : (IActionResult)NotFound();
        }

        //-- GET api/Events/ByEvent/{eventId}
        [HttpGet("ByEvent/{eventId}")]
        [ProducesResponseType(200, Type = typeof(Event))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);

            return @event != null
                ? Ok(@event)
                : (IActionResult)NotFound();
        }

        //-- GET api/Events/ByPartner/{partnerId}
        [HttpGet("ByPartner/{partnerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Event>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllForPartner(Guid partnerId)
        {
            var events = await _eventRepository.GetAllEventsForPartnerAsync(partnerId);

            return events.Any()
                ? Ok(events)
                : (IActionResult)NotFound();
        }

        //-- POST api/Events
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(EventWriteModel))]
        public async Task<IActionResult> Post([BindRequired, FromBody]EventWriteModel eventWriteModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(eventWriteModel);

            var @event = await _eventRepository.SaveEventAsync(eventWriteModel);

            return CreatedAtAction(nameof(Get), new { eventId = @event.EventId }, @event);
        }

        //-- PUT api/Events/{eventId}
        [HttpPut("{eventId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(EventWriteModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(Guid eventId, [BindRequired, FromBody]EventWriteModel eventWriteModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(eventWriteModel);

            var eventUpdated = await _eventRepository.UpdateEventAsync(eventId, eventWriteModel);

            return eventUpdated
                ? NoContent()
                : (IActionResult)NotFound();

        }

        //-- DELETE api/Events/{eventId}
        [HttpDelete("{eventId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            var eventDeleted = await _eventRepository.DeleteEventAsync(eventId);

            return eventDeleted
                ? NoContent()
                : (IActionResult)NotFound();
        }
    }
}
