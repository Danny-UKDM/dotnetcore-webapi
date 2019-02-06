using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class DeleteAllRowsCommand : ICommand
    {
        public IPreparedCommand Prepare(ICommandBuilder commandBuilder)
        {
            return commandBuilder.WithSql("delete from events")
                                 .Build();
        }
    }
}
