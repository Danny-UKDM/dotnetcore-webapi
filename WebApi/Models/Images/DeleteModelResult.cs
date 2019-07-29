using System;

namespace WebApi.Models.Images
{
    public class DeleteModelResult
    {
        public DeleteModelResult(Guid imageId)
        {
            ImageId = imageId;
            Result = ResultStatus.Success;
        }

        public DeleteModelResult(ResultStatus result, string reason = "")
        {
            Result = result;
            Reason = reason;
        }

        public Guid ImageId { get; internal set; }
        public ResultStatus Result { get; }
        public string Reason { get; }
    }
}
