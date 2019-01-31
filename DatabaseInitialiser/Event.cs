using System;

namespace DatabaseInitialiser
{
    internal class Event
    {
        public int EventId { get; set; }
        public Guid PartnerId { get; set; }
        public string EventName { get; set; }
        public string AddressLine1 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
