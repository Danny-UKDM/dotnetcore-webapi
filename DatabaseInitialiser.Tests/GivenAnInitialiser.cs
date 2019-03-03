using System;
using Badger.Data;
using Dapper;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace DatabaseInitialiser.Tests
{
    public class GivenAnInitialiser : IDisposable
    {
        private readonly string _connectionString;
        private readonly Initialiser _initialiser;
        private readonly ISessionFactory _sessionFactory;

        public GivenAnInitialiser()
        {
            _connectionString = "Host=localhost;Username=postgres;Password=password;Pooling=false;Database=testdatabase;";
            _initialiser = new Initialiser(_connectionString);
            _initialiser.Init();

            _sessionFactory = CreateSessionFactory();
        }

        [Fact]
        public void ThenDataCanBeWrittenAndRead()
        {
            InsertTestData();

            using (var session = _sessionFactory.CreateQuerySession())
            {
                session.Execute(new QueryForAllRowsCount()).Should().Be(3);
            }
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString(_connectionString)
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }

        private void InsertTestData()
        {
            var event1 = new EventBuilder().CreateEvent("Cool Event").Build();
            var event2 = new EventBuilder().CreateEvent("Cooler Event").Build();
            var event3 = new EventBuilder().CreateEvent("Coolest Event").Build();

            const string insertSql = @"
insert into events (
    eventId,
    partnerId,
    eventName,
    addressLine1,
    postalCode,
    city,
    country,
    latitude,
    longitude
) values (
    @eventId,
    @partnerId,
    @eventName,
    @addressLine1,
    @postalCode,
    @city,
    @country,
    @latitude,
    @longitude
)";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Execute(insertSql, event1);
                conn.Execute(insertSql, event2);
                conn.Execute(insertSql, event3);
            }
        }

        public void Dispose()
        {
            _initialiser.Dispose();
        }
    }
}
