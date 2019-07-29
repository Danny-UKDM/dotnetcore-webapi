using System;

namespace WebApi.Models.Images
{
    public class CreateModelResult<T> where T : class
    {
        public CreateModelResult(T data)
        {
            Data = data;
            Result = ResultStatus.Success;
        }

        public CreateModelResult(ResultStatus result, string reason = "")
        {
            Data = default;
            Result = result;
            Reason = reason;
        }

        public T Data { get; internal set; }
        public Guid ImageId { get; set; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }
}
