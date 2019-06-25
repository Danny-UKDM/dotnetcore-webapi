using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Events
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

        public DateTime CreatedAt { get; set; }
    }

    public class EventWriteModel
    {
        [Required] [NoGuidEmpty]
        public Guid PartnerId { get; set; }

        [Required] [StringLength(100)]
        public string EventName { get; set; }

        [Required] [StringLength(100)]
        public string AddressLine1 { get; set; }

        [Required] [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required] [StringLength(100)]
        public string City { get; set; }

        [Required] [StringLength(100)]
        public string Country { get; set; }

        [Required] [Range(-90, 90, ErrorMessage = "Invalid Latitude.")]
        public double Latitude { get; set; }

        [Required] [Range(-180, 180, ErrorMessage = "Invalid Longitude.")]
        public double Longitude { get; set; }

        [Required]
        public DateTime OccursOn { get; set; }
    }
}
