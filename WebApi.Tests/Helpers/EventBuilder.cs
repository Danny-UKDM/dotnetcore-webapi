using System;
using WebApi.Models.Events;

namespace WebApi.Tests.Helpers
{
    public class EventBuilder
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

        private EventBuilder(string eventName) => _eventName = eventName;

        public static EventBuilder CreateEvent(string eventName) => new EventBuilder(eventName);

        public EventBuilder WithPartnerId(Guid partnerId)
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
