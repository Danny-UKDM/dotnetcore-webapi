﻿using System;

namespace DatabaseInitialiser.Tests
{
    public class Event
    {
        public Guid EventId { get; set; }
        public Guid PartnerId { get; set; }
        public string EventName { get; set; }
        public string AddressLine1 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime OccursOn { get; set; }
        public DateTime CreatedAt => DateTime.UtcNow;
    }
}
