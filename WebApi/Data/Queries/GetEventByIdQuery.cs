using System;
using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Queries
{
    public class GetEventByIdQuery : IQuery<Event>
    {
        private readonly int _eventId;

        public GetEventByIdQuery(int eventId)
        {
            _eventId = eventId;
        }
        public IPreparedQuery<Event> Prepare(IQueryBuilder builder)
        {
            return builder
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
                    longitude
                    from events
                    where eventId = @eventId")
               .WithParameter("eventId", _eventId)
               .WithSingleMapper(r => new Event
                   {
                       EventId = r.Get<int>("eventId"),
                       PartnerId = r.Get<Guid>("partnerId"),
                       EventName = r.Get<string>("eventName"),
                       AddressLine1 = r.Get<string>("addressLine1"),
                       PostalCode = r.Get<string>("postalCode"),
                       City = r.Get<string>("city"),
                       Country = r.Get<string>("country"),
                       Latitude = r.Get<double>("latitude"),
                       Longitude = r.Get<double>("longitude")
                   })
               .Build();
        }
    }
}
