using System.Collections.Generic;

namespace WebApi.Models
{
    public class ImageModelResult
    {
        public string File { get; set; }
        public IList<ImageModelError> Errors { get; set; }
    }

    public class ImageModelError
    {
        public string Error { get; set; }
    }
}
