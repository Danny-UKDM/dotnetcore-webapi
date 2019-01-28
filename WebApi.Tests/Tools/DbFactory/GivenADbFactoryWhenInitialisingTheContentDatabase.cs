using System;
using System.Data;
using Badger.Data;
using FluentAssertions;
using Xunit;

namespace WebApi.Tests.Tools.DbFactory
{
    public class GivenADbFactoryWhenInitialisingTheContentDatabase : IDisposable
    {
        private readonly WebApi.Tools.DbFactory _dbFactory;
        private readonly ISessionFactory _sessionFactory;

        public GivenADbFactoryWhenInitialisingTheContentDatabase()
        {
            _dbFactory = new WebApi.Tools.DbFactory("testdatabase");
            _dbFactory.InitDatabase();
            _sessionFactory = _dbFactory.CreateSessionFactory();
        }

        [Fact]
        public void ThenTheDbConnectionIsOpen()
        {
            _dbFactory.Connection.State.Should().Be(ConnectionState.Open);
        }

        [Fact]
        public void ThenTestDataExistsInTheEventsTable()
        {
            using (var session = _sessionFactory.CreateQuerySession())
            {
                session.Execute(new QueryForAllRowsCount()).Should().Be(3);
            }
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
            _dbFactory.Dispose();
        }
    }
}
