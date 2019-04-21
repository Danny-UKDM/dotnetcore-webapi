using Badger.Data;
using WebApi.Models;

namespace WebApi.IntegrationTests.Data
{
    internal class InsertEventCommand : ICommand
    {
        public Event Event { get; }

        public InsertEventCommand(Event @event) => Event = @event;

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
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
 )")
                .WithParameter("eventId", Event.EventId)
                .WithParameter("partnerId", Event.PartnerId)
                .WithParameter("eventName", Event.EventName)
                .WithParameter("addressLine1", Event.AddressLine1)
                .WithParameter("postalCode", Event.PostalCode)
                .WithParameter("city", Event.City)
                .WithParameter("country", Event.Country)
                .WithParameter("latitude", Event.Latitude)
                .WithParameter("longitude", Event.Longitude)
                .Build();
    }
}
