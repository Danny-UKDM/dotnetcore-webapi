﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Badger.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApi.Data.Commands;
using WebApi.Data.Queries;
using WebApi.Models.Events;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ISessionFactory _sessionFactory;

        public EventsController(ISessionFactory sessionFactory) =>
            _sessionFactory = sessionFactory;

        //-- GET api/Events/All
        [HttpGet("All")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Event>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAll()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var events = await session.ExecuteAsync(new GetAllEventsQuery());
                return events.Any()
                    ? Ok(events)
                    : (IActionResult)NotFound();
            }
        }

        //-- GET api/Events/ByEvent/{eventId}
        [HttpGet("ByEvent/{eventId}")]
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

        //-- GET api/Events/ByPartner/{partnerId}
        [HttpGet("ByPartner/{partnerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Event>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllForPartner(Guid partnerId)
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                var events = await session.ExecuteAsync(new GetEventsByPartnerIdQuery(partnerId));

                return events.Any()
                    ? Ok(events)
                    : (IActionResult)NotFound();
            }
        }

        //-- POST api/Events
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(EventWriteModel))]
        public async Task<IActionResult> Post([BindRequired, FromBody]EventWriteModel eventWriteModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(eventWriteModel);

            var insertEventCommand = new InsertEventCommand(eventWriteModel);
            using (var session = _sessionFactory.CreateCommandSession())
            {
                await session.ExecuteAsync(insertEventCommand);
                session.Commit();
            }

            return CreatedAtAction(nameof(Get), new { eventId = insertEventCommand.Event.EventId }, insertEventCommand.Event);
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

            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new UpdateEventCommand(eventId, eventWriteModel));
                session.Commit();

                return affected != 0
                    ? NoContent()
                    : (IActionResult)NotFound();
            }
        }

        //-- DELETE api/Events/{eventId}
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
