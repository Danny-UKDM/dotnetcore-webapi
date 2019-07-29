using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Badger.Data;
using WebApi.Data.Commands;
using WebApi.Data.Queries;
using WebApi.Models.Events;

namespace WebApi.Data
{
    public class EventRepository : IEventRepository
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly MemCache<Event> _eventCache;
        private readonly MemCache<IEnumerable<Event>> _eventsCache;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        public EventRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _eventCache = new MemCache<Event>();
            _eventsCache = new MemCache<IEnumerable<Event>>();
        }

        public Task<Event> GetEventByIdAsync(Guid eventId) =>
            _eventCache.Get(eventId.ToString(), async () => await GetEventById(eventId), _timeout);

        private async Task<Event> GetEventById(Guid eventId)
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                return await session.ExecuteAsync(new GetEventByIdQuery(eventId));
            }
        }

        public Task<IEnumerable<Event>> GetAllEventsForPartnerAsync(Guid partnerId) =>
            _eventsCache.Get(partnerId.ToString(), async () => await GetAllEventsForPartner(partnerId), _timeout);

        private async Task<IEnumerable<Event>> GetAllEventsForPartner(Guid partnerId)
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                return await session.ExecuteAsync(new GetEventsByPartnerIdQuery(partnerId));
            }
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                return await session.ExecuteAsync(new GetAllEventsQuery());
            }
        }

        public async Task<Event> SaveEventAsync(EventWriteModel eventWriteModel)
        {
            var insertEventCommand = new InsertEventCommand(eventWriteModel);
            using (var session = _sessionFactory.CreateCommandSession())
            {
                await session.ExecuteAsync(insertEventCommand);
                session.Commit();
            }

            return insertEventCommand.Event;
        }

        public async Task<bool> UpdateEventAsync(Guid eventId, EventWriteModel eventWriteModel)
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new UpdateEventCommand(eventId, eventWriteModel));
                session.Commit();

                return affected != 0;
            }
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            using (var session = _sessionFactory.CreateCommandSession())
            {
                var affected = await session.ExecuteAsync(new DeleteEventCommand(eventId));
                session.Commit();

                return affected != 0;
            }
        }
    }
}
