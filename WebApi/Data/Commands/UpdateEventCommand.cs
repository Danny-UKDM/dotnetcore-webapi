using System;
using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class UpdateEventCommand : ICommand
    {
        public Guid EventId { get; }
        public Event Details { get; }

        public UpdateEventCommand(Guid eventId, Event details)
        {
            EventId = eventId;
            Details = details;
        }
        
        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
update events set
    eventId = @eventId,
    partnerId = @partnerId,
    eventName = @eventName,
    addressLine1 = @addressLine1,
    postalCode = @postalCode,
    city = @city,
    country = @country,
    latitude = @latitude,
    longitude = @longitude
where eventId = @existingEventId")
                .WithParameter("existingEventId", EventId)
                .WithParameter("eventId", Details.EventId)
                .WithParameter("partnerId", Details.PartnerId)
                .WithParameter("eventName", Details.EventName)
                .WithParameter("addressLine1", Details.AddressLine1)
                .WithParameter("postalCode", Details.PostalCode)
                .WithParameter("city", Details.City)
                .WithParameter("country", Details.Country)
                .WithParameter("latitude", Details.Latitude)
                .WithParameter("longitude", Details.Longitude)
                .Build();
    }
}
