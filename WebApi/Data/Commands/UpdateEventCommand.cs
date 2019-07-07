using System;
using Badger.Data;
using WebApi.Models.Events;

namespace WebApi.Data.Commands
{
    internal class UpdateEventCommand : ICommand
    {
        public Guid EventId { get; internal set; }
        public EventWriteModel EventWriteModel { get; internal set; }

        public UpdateEventCommand(Guid eventId, EventWriteModel eventWriteModel)
        {
            EventId = eventId;
            EventWriteModel = eventWriteModel;
        }

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
update events set
    eventName = @eventName,
    addressLine1 = @addressLine1,
    postalCode = @postalCode,
    city = @city,
    country = @country,
    latitude = @latitude,
    longitude = @longitude,
    occursOn = @occursOn
where eventId = @eventId")
                .WithParameter("eventId", EventId)
                .WithParameter("partnerId", EventWriteModel.PartnerId)
                .WithParameter("eventName", EventWriteModel.EventName)
                .WithParameter("addressLine1", EventWriteModel.AddressLine1)
                .WithParameter("postalCode", EventWriteModel.PostalCode)
                .WithParameter("city", EventWriteModel.City)
                .WithParameter("country", EventWriteModel.Country)
                .WithParameter("latitude", EventWriteModel.Latitude)
                .WithParameter("longitude", EventWriteModel.Longitude)
                .WithParameter("occursOn", EventWriteModel.OccursOn)
                .Build();
    }
}
