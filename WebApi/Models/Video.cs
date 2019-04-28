﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Video
    {
        [Required] public Guid VideoId { get; set; }
        [Required] public Guid PartnerId { get; set; }
        [Required] [StringLength(100)] public string VideoName { get; set; }
        [Required] [StringLength(50)] public string Url { get; set; }
    }
}