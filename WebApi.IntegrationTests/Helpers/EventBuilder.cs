using System;
using WebApi.Models;

namespace WebApi.IntegrationTests.Helpers
{
    public class EventBuilder
    {
        private readonly Event _event;

        public EventBuilder()
        {
            _event = new Event
            {
                EventId = Guid.NewGuid(),
                PartnerId = Guid.NewGuid(),
                EventName = "Some Event Name",
                AddressLine1 = "Some Address",
                PostalCode = "NG71FB",
                City = "Some City",
                Country = "Some Country",
                Latitude = new Random().Next(-90, 90),
                Longitude = new Random().Next(-180, 180)
            };
        }

        public EventBuilder CreateEvent(string eventName)
        {
            _event.EventName = eventName;
            return this;
        }

        public EventBuilder InCity(string city)
        {
            _event.City = city;
            return this;
        }

        public Event Build()
        {
            return _event;
        }
    }
}
