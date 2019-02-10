using System;
using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class DeleteRowsByEventIdCommand : ICommand
    {
        private readonly Guid[] _eventIds;

        public DeleteRowsByEventIdCommand(Guid[] eventIds)
        {
            _eventIds = eventIds;
        }
        public IPreparedCommand Prepare(ICommandBuilder commandBuilder)
        {
            return commandBuilder.WithSql(@"delete from events 
                                    where eventId = any(@eventIds)")
                                 .WithParameter("eventIds", _eventIds)
                                 .Build();
        }
    }
}
