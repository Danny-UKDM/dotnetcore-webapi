using System;
using Badger.Data;

namespace WebApi.Data.Commands
{
    public class DeleteVideoCommand : ICommand
    {
        public Guid VideoId { get; }

        public DeleteVideoCommand(Guid videoId) => VideoId = videoId;

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
delete from videos
where videoId = @videoId")
                .WithParameter("videoId", VideoId)
                .Build();
    }
}
