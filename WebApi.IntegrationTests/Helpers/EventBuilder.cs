using System;
using WebApi.Models.Events;

namespace WebApi.IntegrationTests.Helpers
{
    public class EventBuilder
    {
        private readonly Event _event;

        private EventBuilder(string eventName) =>
            _event = new Event
            {
                EventId = Guid.NewGuid(),
                PartnerId = Guid.NewGuid(),
                EventName = eventName,
                AddressLine1 = "Some Address",
                PostalCode = "NG71FB",
                City = "Some City",
                Country = "Some Country",
                Latitude = new Random().Next(-90, 90),
                Longitude = new Random().Next(-180, 180),
                OccursOn = DateTime.UtcNow.AddDays(new Random().Next(365)),
                CreatedAt = DateTime.UtcNow
            };

        public static EventBuilder CreateEvent(string eventName) =>
            new EventBuilder(eventName);

        public EventBuilder InCity(string city)
        {
            _event.City = city;
            return this;
        }

        public Event Build() => _event;
    }

    public static class EventBuilderExtensions
    {
        public static EventWriteModel ToEventWriteModel(this Event @event) =>
            new EventWriteModel
            {
                PartnerId = @event.PartnerId,
                EventName = @event.EventName,
                AddressLine1 = @event.AddressLine1,
                PostalCode = @event.PostalCode,
                City = @event.City,
                Country = @event.Country,
                Latitude = @event.Latitude,
                Longitude = @event.Longitude,
                OccursOn = @event.OccursOn,
            };
    }
}
