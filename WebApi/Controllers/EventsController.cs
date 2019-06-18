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

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ISessionFactory _sessionFactory;

        public EventsController(ISessionFactory sessionFactory) =>
            _sessionFactory = sessionFactory;

        //-- GET api/events
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Event>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var events = await session.ExecuteAsync(new GetAllEventsQuery());
                return events.Any()
                    ? Ok(events)
                    : (IActionResult)NotFound();
            }
        }

        //-- GET api/events/{eventId}
        [HttpGet("{eventId}")]
        [ProducesResponseType(200, Type = typeof(Event))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid eventId)
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var @event = await session.ExecuteAsync(new GetEventByIdQuery(eventId));

                return @event != null
                    ? Ok(@event)
                    : (IActionResult)NotFound();
            }
        }

        //-- POST api/events
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(Event))]
        public async Task<IActionResult> Post([BindRequired, FromBody]Event @event)
        {
            if (!ModelState.IsValid)
                return BadRequest(@event);

            using (var session = _sessionFactory.CreateCommandSession())
            {
                await session.ExecuteAsync(new InsertEventCommand(@event));
                session.Commit();
            }

            return CreatedAtAction(nameof(Get), new { eventId = @event.EventId }, @event);
        }

        //-- PUT api/events/{eventId}
        [HttpPut("{eventId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(Event))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(Guid eventId, [BindRequired, FromBody]Event @event)
        {
            if (!ModelState.IsValid)
                return BadRequest(@event);

            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new UpdateEventCommand(eventId, @event));
                session.Commit();

                return affected != 0
                    ? NoContent()
                    : (IActionResult)NotFound();
            }
        }

        //-- DELETE api/events/{eventId}
        [HttpDelete("{eventId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new DeleteEventCommand(eventId));
                session.Commit();

                return affected != 0
                    ? NoContent()
                    : (IActionResult)NotFound();
            }
        }
    }
}
