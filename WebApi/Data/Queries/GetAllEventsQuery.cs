using System;
using System.Collections.Generic;
using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Queries
{
    internal class GetAllEventsQuery : IQuery<IEnumerable<Event>>
    {
        public IPreparedQuery<IEnumerable<Event>> Prepare(IQueryBuilder builder) =>
            builder
                .WithSql(@"
select
    eventId,
    partnerId,
    eventName,
    addressLine1,
    postalCode,
    city,
    country,
    latitude,
    longitude,
    createdAt,
    occursOn
from events")
                .WithMapper(r => new Event
                {
                    EventId = r.Get<Guid>("eventId"),
                    PartnerId = r.Get<Guid>("partnerId"),
                    EventName = r.Get<string>("eventName"),
                    AddressLine1 = r.Get<string>("addressLine1"),
                    PostalCode = r.Get<string>("postalCode"),
                    City = r.Get<string>("city"),
                    Country = r.Get<string>("country"),
                    Latitude = r.Get<double>("latitude"),
                    Longitude = r.Get<double>("longitude"),
                    CreatedAt = r.Get<DateTime>("createdAt"),
                    OccursOn = r.Get<DateTime>("occursOn")
                })
                .Build();
    }
}
