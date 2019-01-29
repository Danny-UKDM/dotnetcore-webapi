using System;
using System.Data;
using Badger.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Npgsql;
using NSubstitute;
using Xunit;

namespace WebApi.Tests.Tools.DbInitialiser
{
    public class GivenADbInitialiser : IDisposable
    {
        private readonly WebApi.Tools.DbInitialiser _dbInitialiser;
        private readonly ISessionFactory _sessionFactory;
        private const string Database = "testdatabase";

        public GivenADbInitialiser()
        {
            var logger = Substitute.For<ILogger>();
            _dbInitialiser = new WebApi.Tools.DbInitialiser(Database, logger);
            _dbInitialiser.Init();
            _sessionFactory = CreateSessionFactory();
        }

        [Fact]
        public void ThenTheDbConnectionIsOpen()
        {
            _dbInitialiser.Connection.State.Should().Be(ConnectionState.Open);
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
                config.WithConnectionString($"Host=localhost;Username=postgres;Password=password;Pooling=false;Database={Database}")
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
            _dbInitialiser.Dispose();
        }
    }
}
