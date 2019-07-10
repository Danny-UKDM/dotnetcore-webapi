using System;
using WebApi.Models.Events;

namespace WebApi.Tests.Helpers
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

        public EventBuilder WithPartnerId(Guid partnerId)
        {
            _event.PartnerId = partnerId;
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

    public class EventBuilderV2
    {
        private static readonly int Ticks = (int)DateTime.UtcNow.Ticks;
        private readonly Guid _eventId = Guid.NewGuid();
        private Guid _partnerId = Guid.NewGuid();
        private readonly string _eventName;
        private const string AddressLine1 = "Some Address";
        private const string PostalCode = "NG71FB";
        private const string City = "Some City";
        private const string Country = "Some Country";
        private readonly double _latitude = new Random(Ticks).Next(-90, 90);
        private readonly double _longitude = new Random(Ticks).Next(-80, 180);
        private readonly DateTime _occursOn = DateTime.UtcNow.AddDays(new Random(Ticks).Next(365));
        private readonly DateTime _createdAt = DateTime.UtcNow;

        private EventBuilderV2(string eventName) => _eventName = eventName;

        public static EventBuilderV2 CreateEvent(string eventName) => new EventBuilderV2(eventName);

        public EventBuilderV2 WithPartnerId(Guid partnerId)
        {
            _partnerId = partnerId;
            return this;
        }

        public (Event Event, EventWriteModel EventWriteModel) Build()
        {
            return (
                Event: new Event
                {
                    EventId = _eventId,
                    PartnerId = _partnerId,
                    EventName = _eventName,
                    AddressLine1 = AddressLine1,
                    PostalCode = PostalCode,
                    City = City,
                    Country = Country,
                    Latitude = _latitude,
                    Longitude = _longitude,
                    OccursOn = _occursOn,
                    CreatedAt = _createdAt
                },
                EventWriteModel: new EventWriteModel
                {
                    PartnerId = _partnerId,
                    EventName = _eventName,
                    AddressLine1 = AddressLine1,
                    PostalCode = PostalCode,
                    City = City,
                    Country = Country,
                    Latitude = _latitude,
                    Longitude = _longitude,
                    OccursOn = _occursOn
                });
        }
    }
}
