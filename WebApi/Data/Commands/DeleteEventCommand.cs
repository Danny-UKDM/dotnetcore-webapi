using System;
using Badger.Data;

namespace WebApi.Data.Commands
{
    public class DeleteEventCommand : ICommand
    {
        public Guid EventId { get; }

        public DeleteEventCommand(Guid eventId) => EventId = eventId;

        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"
delete from events
where eventId = @eventId")
                .WithParameter("eventId", EventId)
                .Build();
    }
}
