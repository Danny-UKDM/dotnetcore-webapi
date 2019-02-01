using System;
using System.Data;
using Badger.Data;
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
        public void ThenTheDbConnectionIsOpen()
        {
            _initialiser.Connection.State.Should().Be(ConnectionState.Open);
        }

        [Fact]
        public void ThenTestDataExistsInTheEventsTable()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                session.Execute(new QueryForAllRowsCount()).Should().Be(3);
            }
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString($"Host=localhost;Username=postgres;Password=password;Pooling=false;Port=5433;Database={Database}")
                   .WithProviderFactory(NpgsqlFactory.Instance));
        }

        private class QueryForAllRowsCount : IQuery<long>
        {
            public IPreparedQuery<long> Prepare(IQueryBuilder queryBuilder)
            {
                return queryBuilder.WithSql("select count(*) from events")
                                   .WithScalar<long>()
                                   .Build();
            }
        }

        public void Dispose()
        {
            _initialiser.Dispose();
        }
    }
}
