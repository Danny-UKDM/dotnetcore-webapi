using System;

namespace WebApi.Models.Events
{
    public static class EventExtensions
    {
        public static Event ToEvent(this EventWriteModel eventWriteModel) =>
            new Event
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
                OccursOn = eventWriteModel.OccursOn,
                CreatedAt = DateTime.UtcNow
            };
    }
}
