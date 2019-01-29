using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Badger.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApi.Data.Queries;
using WebApi.Models;

namespace WebApi.Services
{
    public class EventRepository : IEventRepository
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
                _logger.LogError("Error retrieving all events", ex);
            }

            return events;
        }

        public Task<Event> GetEventByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString("Host=localhost;Username=postgres;Password=password;Pooling=false;Database=content")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }
    }
}
