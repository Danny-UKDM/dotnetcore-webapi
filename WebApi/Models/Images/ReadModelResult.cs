using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Models.Images
{
    public class ReadModelResult
    {
        public ReadModelResult(Guid imageId)
        {
            ImageId = imageId;
            Result = ResultStatus.Success;
        }

        public ReadModelResult(ResultStatus result, string reason = "")
        {
            Result = result;
            Reason = reason;
        }

        [JsonProperty]
        public IEnumerable<string> Locations { get; set; }
        public Guid ImageId { get; internal set; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }
}
