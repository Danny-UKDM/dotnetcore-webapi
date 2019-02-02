using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class UpdateEventCommand : ICommand
    {
        private readonly Event _event;

        public UpdateEventCommand(Event @event)
        {
            _event = @event;
        }
        public IPreparedCommand Prepare(ICommandBuilder builder)
        {
            return builder
               .WithSql(@"update events set
                    partnerId = @partnerId,
                    eventName = @eventName,
                    addressLine1 = @addressLine1,
                    postalCode = @postalCode,
                    city = @city,
                    country = @country,
                    latitude = @latitude,
                    longitude = @longitude
                    where eventId = @eventId")
               .WithParameter("eventId", _event.EventId)
               .WithParameter("partnerId", _event.PartnerId)
               .WithParameter("eventName", _event.EventName)
               .WithParameter("addressLine1", _event.AddressLine1)
               .WithParameter("postalCode", _event.PostalCode)
               .WithParameter("city", _event.City)
               .WithParameter("country", _event.Country)
               .WithParameter("latitude", _event.Latitude)
               .WithParameter("longitude", _event.Longitude)
               .Build();
        }
    }
}
