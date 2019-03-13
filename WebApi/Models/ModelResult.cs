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

        public ModelResult(string imageId, string contentType)
        {
            Data = default(T);
            ImageId = imageId;
            ContentType = contentType;
            Result = ResultStatus.Success;
        }

        public ModelResult(ResultStatus result, string reason = "", T data = default(T), string contentType = "")
        {
            Result = result;
            Data = data;
            Reason = reason;
            ContentType = contentType;
        }

        public T Data { get; set; }
        public string ImageId { get; set; }
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
