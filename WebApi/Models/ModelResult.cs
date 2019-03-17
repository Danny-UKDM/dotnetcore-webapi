using System;

namespace WebApi.Models
{
    public class ModelResult<T> where T : class
    {
        public ModelResult(T data)
        {
            Data = data;
            Result = ResultStatus.Success;
        }

        public ModelResult(Guid imageId)
        {
            Data = default(T);
            ImageId = imageId;
            Result = ResultStatus.Success;
        }

        public ModelResult(ResultStatus result, string reason = "")
        {
            Result = result;
            Data = default(T);
            Reason = reason;
        }

        public T Data { get; set; }
        public Guid ImageId { get; internal set; }
        public string ContentType { get; set; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }

    public enum ResultStatus
    {
        Failed,
        Success
    }
}
