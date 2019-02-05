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
        private readonly ISessionFactory _sessionFactory;
        private readonly Initialiser _initialiser;
        private const string Database = "testdatabase";

        public GivenAnInitialiser()
        {
            _initialiser = new Initialiser(Database);
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
                config.WithConnectionString(_initialiser.ConnectionString)
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }

        protected void InsertTestData()
        {
            var event1 = new EventBuilder().CreateEvent("Cool Event").Build();
            var event2 = new EventBuilder().CreateEvent("Cooler Event").Build();
            var event3 = new EventBuilder().CreateEvent("Coolest Event").Build();

            const string insertSql = @"insert into events (
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
                    @longitude )";

            using (var conn = new NpgsqlConnection(_initialiser.ConnectionString))
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
