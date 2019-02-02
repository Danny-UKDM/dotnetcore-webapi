using System;
using Badger.Data;

namespace WebApi.Data.Commands
{
    public class DeleteEventCommand : ICommand
    {
        private readonly Guid _eventId;

        public DeleteEventCommand(Guid eventId)
        {
            _eventId = eventId;
        }
        public IPreparedCommand Prepare(ICommandBuilder builder)
        {
            return builder
               .WithSql(@"delete from events
                    where eventId = @eventId")
               .WithParameter("eventId", _eventId)
               .Build();
        }
    }
}
