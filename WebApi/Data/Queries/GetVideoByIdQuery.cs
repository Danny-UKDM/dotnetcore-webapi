using System;
using Badger.Data;
using WebApi.Models;
using WebApi.Models.Videos;

namespace WebApi.Data.Queries
{
    internal class GetVideoByIdQuery : IQuery<Video>
    {
        public Guid VideoId { get; }

        public GetVideoByIdQuery(Guid videoId) => VideoId = videoId;

        public IPreparedQuery<Video> Prepare(IQueryBuilder builder) =>
            builder
                .WithSql(@"
select
    videoId,
    partnerId,
    videoName,
    url
from videos
where videoId = @videoId")
                .WithParameter("videoId", VideoId)
                .WithSingleMapper(r => new Video
                {
                    VideoId = r.Get<Guid>("videoId"),
                    PartnerId = r.Get<Guid>("partnerId"),
                    VideoName = r.Get<string>("videoName"),
                    Url = r.Get<string>("url")
                })
                .Build();
    }
}
