using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        // GET api/events
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var events = await _eventRepository.GetAllEventsAsync();

            return new JsonResult(events);
        }

        // GET api/events/{eventId}
        [HttpGet("{eventId}")]
        public async Task<ActionResult<string>> Get(int eventId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);

            return new JsonResult(@event);
        }

        //// POST api/events
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

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
