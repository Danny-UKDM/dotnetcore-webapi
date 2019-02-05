using System;
using System.Data;
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
        private readonly EventBuilder _eventBuilder;
        private const string Database = "testdatabase";

        public GivenAnInitialiser()
        {
            _eventBuilder = new EventBuilder();

            _sessionFactory = CreateSessionFactory();
            _initialiser = new Initialiser(Database);
            _initialiser.Init();
        }

        [Fact]
        public void ThenTheDbConnectionIsOpen()
        {
            _initialiser.Connection.State.Should().Be(ConnectionState.Open);
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
                config.WithConnectionString($"Host=localhost;Username=postgres;Password=password;Pooling=false;Database={Database}")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }

        protected void InsertTestData()
        {
            var event1 = _eventBuilder.CreateEvent("Cool Event").Build();
            var event2 = _eventBuilder.CreateEvent("Cooler Event").Build();
            var event3 = _eventBuilder.CreateEvent("Coolest Event").Build();

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
            _initialiser.Connection.Execute(insertSql, event1);
            _initialiser.Connection.Execute(insertSql, event2);
            _initialiser.Connection.Execute(insertSql, event3);
        }

        public void Dispose()
        {
            _initialiser.Dispose();
        }
    }
}
