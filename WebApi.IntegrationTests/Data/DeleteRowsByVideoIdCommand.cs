using System;
using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class DeleteRowsByVideoIdCommand : ICommand
    {
        private readonly Guid[] _videoIds;

        public DeleteRowsByVideoIdCommand(Guid[] videoIds) =>
            _videoIds = videoIds;

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
delete from videos
where videoId = any(@videoIds)")
                .WithParameter("videoIds", _videoIds)
                .Build();
    }
}
