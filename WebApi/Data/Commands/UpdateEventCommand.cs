using System;
using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class UpdateEventCommand : ICommand
    {
        private readonly Event _newEvent;
        private readonly Guid _existingEventId;

        public UpdateEventCommand(Event newNewEvent, Guid existingEventId)
        {
            _newEvent = newNewEvent;
            _existingEventId = existingEventId;
        }
        public IPreparedCommand Prepare(ICommandBuilder builder)
        {
            return builder
               .WithSql(@"update events set
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
               .WithParameter("existingEventId", _existingEventId)
               .WithParameter("eventId", _newEvent.EventId)
               .WithParameter("partnerId", _newEvent.PartnerId)
               .WithParameter("eventName", _newEvent.EventName)
               .WithParameter("addressLine1", _newEvent.AddressLine1)
               .WithParameter("postalCode", _newEvent.PostalCode)
               .WithParameter("city", _newEvent.City)
               .WithParameter("country", _newEvent.Country)
               .WithParameter("latitude", _newEvent.Latitude)
               .WithParameter("longitude", _newEvent.Longitude)
               .Build();
        }
    }
}
