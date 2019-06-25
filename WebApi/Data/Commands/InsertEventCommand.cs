using System;
using Badger.Data;
using WebApi.Models.Events;

namespace WebApi.Data.Commands
{
    internal class InsertEventCommand : ICommand
    {
        public EventWriteModel EventWriteModel { get; internal set; }
        public Event Event { get; internal set; }

        public InsertEventCommand(EventWriteModel eventWriteModel)
        {
            var @event = new Event
            {
                EventId = Guid.NewGuid(),
                PartnerId = eventWriteModel.PartnerId,
                EventName = eventWriteModel.EventName,
                AddressLine1 = eventWriteModel.AddressLine1,
                PostalCode = eventWriteModel.PostalCode,
                City = eventWriteModel.City,
                Country = eventWriteModel.Country,
                Latitude = eventWriteModel.Latitude,
                Longitude = eventWriteModel.Longitude,
                CreatedAt = DateTime.UtcNow,
                OccursOn = eventWriteModel.OccursOn,
            };

            EventWriteModel = eventWriteModel;
            Event = @event;
        }

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
    longitude,
    createdAt,
    occursOn
 ) values (
    @eventId,
    @partnerId,
    @eventName,
    @addressLine1,
    @postalCode,
    @city,
    @country,
    @latitude,
    @longitude,
    @createdAt,
    @occursOn
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
                .WithParameter("createdAt", Event.CreatedAt)
                .WithParameter("occursOn", Event.OccursOn)
                .Build();
    }
}
