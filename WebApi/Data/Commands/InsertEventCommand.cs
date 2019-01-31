using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class InsertEventCommand : ICommand
    {
        private readonly Event _event;

        public InsertEventCommand(Event @event)
        {
            _event = @event;
        }
        public IPreparedCommand Prepare(ICommandBuilder builder)
        {
            return builder
               .WithSql(@"insert into events (
                    @eventId,
                    @partnerId,
                    @eventName,
                    @addressLine1,
                    @postalCode,
                    @city,
                    @country,
                    @latitude,
                    @longitude )")
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

