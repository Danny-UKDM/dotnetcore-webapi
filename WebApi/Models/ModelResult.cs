using System;

namespace WebApi.Models
{
    public class ModelResult<T> where T : class
    {
        public ModelResult(T data)
        {
            Data = data;
            ImageId = Guid.NewGuid();
            Result = ResultStatus.Success;
        }

        public ModelResult(ResultStatus result, string reason = "", T data = default(T))
        {
            Result = result;
            Data = data;
            Reason = reason;
        }

        public T Data { get; }
        public Guid ImageId { get; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }

    public enum ResultStatus
    {
        Failed,
        Success
    }
}
