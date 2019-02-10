﻿using System;
using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class CountRowsByEventIdsQuery : IQuery<long>
    {
        private readonly Guid[] _eventIds;

        public CountRowsByEventIdsQuery(Guid[] eventIds)
        {
            _eventIds = eventIds;
        }
        public IPreparedQuery<long> Prepare(IQueryBuilder queryBuilder)
        {
            return queryBuilder.WithSql(@"select count(*) from events
                                    where eventId = any(@eventIds)")
                               .WithParameter("eventIds", _eventIds)
                               .WithScalar<long>()
                               .Build();
        }
    }
}
