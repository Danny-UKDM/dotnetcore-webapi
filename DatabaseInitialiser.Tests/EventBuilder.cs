﻿using System;

namespace DatabaseInitialiser.Tests
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
                OccursOn = DateTime.UtcNow.AddDays(new Random().Next(365))
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
}
