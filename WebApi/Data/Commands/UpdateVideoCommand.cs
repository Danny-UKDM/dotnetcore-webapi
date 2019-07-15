using System;
using Badger.Data;
using WebApi.Models;
using WebApi.Models.Videos;

namespace WebApi.Data.Commands
{
    internal class UpdateVideoCommand : ICommand
    {
        public Guid VideoId { get; }
        public Video Details { get; }

        public UpdateVideoCommand(Guid videoId, Video details)
        {
            VideoId = videoId;
            Details = details;
        }
        
        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
update videos set
    partnerId = @partnerId,
    videoName = @videoName,
    url = @url
where videoId = @videoId")
                .WithParameter("videoId", VideoId)
                .WithParameter("partnerId", Details.PartnerId)
                .WithParameter("videoName", Details.VideoName)
                .WithParameter("url", Details.Url)
                .Build();
    }
}
