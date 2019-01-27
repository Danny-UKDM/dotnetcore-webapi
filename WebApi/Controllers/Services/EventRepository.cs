using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Badger.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApi.Data.Queries;
using WebApi.Models;
using WebApi.Tools;

namespace WebApi.Controllers.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly ISessionFactory _sessionFactory;
        private ILogger _logger = ApplicationLogging.CreateLogger<EventRepository>();

        public EventRepository()
        {
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
                config.WithConnectionString("Host=localhost;Username=postgres;Password=password;Pooling=false;Port=5433;Database=content")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }
    }
}
