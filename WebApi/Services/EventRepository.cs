using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Badger.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApi.Data.Commands;
using WebApi.Data.Queries;
using WebApi.Models;

namespace WebApi.Services
{
    internal class EventRepository : IEventRepository
    {
        private readonly ILogger<EventRepository> _logger;
        private readonly ISessionFactory _sessionFactory;

        public EventRepository(ILogger<EventRepository> logger)
        {
            _logger = logger;
            _sessionFactory = CreateSessionFactory();
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            IEnumerable<Event> events = new List<Event>();

            try
            {
                using (var session = _sessionFactory.CreateQuerySession())
                {
                    events = await session.ExecuteAsync(new GetAllEventsQuery());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all events: {ex.Message}");
            }

            return events;
        }

        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            Event @event = new Event();

            try
            {
                using (var session = _sessionFactory.CreateQuerySession())
                {
                    @event = await session.ExecuteAsync(new GetEventByIdQuery(eventId));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving event {eventId}: {ex.Message}");
            }

            return @event;
        }

        public async Task AddEventAsync(Event @event)
        {
            try
            {
                using (var session = _sessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new InsertEventCommand(@event));

                    session.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding event {@event.EventId}: {ex.Message}");
            }
        }

        public async Task UpdateEventAsync(Event @event)
        {
            try
            {
                using (var session = _sessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new UpdateEventCommand(@event));

                    session.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating event {@event.EventId}: {ex.Message}");
            }
        }

        public async Task DeleteEventAsync(Guid eventId)
        {
            try
            {
                using (var session = _sessionFactory.CreateCommandSession())
                {
                    await session.ExecuteAsync(new DeleteEventCommand(eventId));

                    session.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting event {eventId}: {ex.Message}");
            }
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString("Host=localhost;Username=postgres;Password=password;Pooling=false;Database=content")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }
    }
}
