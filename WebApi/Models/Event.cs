﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Event
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid PartnerId { get; set; }
        [Required]
        [StringLength(100)]
        public string EventName { get; set; }
        [Required]
        [StringLength(100)]
        public string AddressLine1 { get; set; }
        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; }
        [Required]
        [Range(-90, 90, ErrorMessage = "Invalid Latitude.")]
        public double Latitude { get; set; }
        [Required]
        [Range(-180, 180, ErrorMessage = "Invalid Longitude.")]
        public double Longitude { get; set; }
    }
}