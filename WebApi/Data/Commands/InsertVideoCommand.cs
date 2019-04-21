using Badger.Data;
using WebApi.Models;

namespace WebApi.Data.Commands
{
    internal class InsertVideoCommand : ICommand
    {
        public Video Video { get; }

        public InsertVideoCommand(Video video) => Video = video;

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
insert into videos (
    videoId,
    partnerId,
    videoName,
    url
 ) values (
    @videoId,
    @partnerId,
    @videoName,
    @url
 )")
                .WithParameter("videoId", Video.VideoId)
                .WithParameter("partnerId", Video.PartnerId)
                .WithParameter("videoName", Video.VideoName)
                .WithParameter("url", Video.Url)
                .Build();
    }
}
