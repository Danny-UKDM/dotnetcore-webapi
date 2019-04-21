using System;
using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class UpdateVideoCommand : ICommand
    {
        public Guid EventId { get; }
        public Video Details { get; }

        public UpdateVideoCommand(Guid videoId, Video details)
        {
            EventId = videoId;
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
                .WithParameter("videoId", Details.VideoId)
                .WithParameter("partnerId", Details.PartnerId)
                .WithParameter("videoName", Details.VideoName)
                .WithParameter("url", Details.Url)
                .Build();
    }
}
