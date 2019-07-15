using System;
using System.Collections.Generic;
using Badger.Data;
using WebApi.Models;
using WebApi.Models.Videos;

namespace WebApi.Data.Queries
{
    internal class GetAllVideosQuery : IQuery<IEnumerable<Video>>
    {
        public IPreparedQuery<IEnumerable<Video>> Prepare(IQueryBuilder builder) =>
            builder
                .WithSql(@"
select
    videoId,
    partnerId,
    videoName,
    url
from videos")
                .WithMapper(r => new Video
                {
                    VideoId = r.Get<Guid>("videoId"),
                    PartnerId = r.Get<Guid>("partnerId"),
                    VideoName = r.Get<string>("videoName"),
                    Url = r.Get<string>("url")
                })
                .Build();
    }
}
