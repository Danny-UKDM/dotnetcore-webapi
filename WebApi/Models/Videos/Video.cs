using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Videos
{
    public class Video
    {
        [Required] [NoGuidEmpty] public Guid VideoId { get; set; }
        [Required] [NoGuidEmpty] public Guid PartnerId { get; set; }
        [Required] [StringLength(100)] public string VideoName { get; set; }
        [Required] [StringLength(50)] public string Url { get; set; }
    }
}
