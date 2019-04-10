using System;

namespace WebApi.Models
{
    public class ReadModelResult<T> where T : class
    {
        public ReadModelResult(Guid imageId)
        {
            Data = default;
            ImageId = imageId;
            Result = ResultStatus.Success;
        }

        public ReadModelResult(ResultStatus result, string reason = "")
        {
            Data = default;
            Result = result;
            Reason = reason;
        }

        public T Data { get; set; }
        public Guid ImageId { get; internal set; }
        public string ContentType { get; set; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }
}
