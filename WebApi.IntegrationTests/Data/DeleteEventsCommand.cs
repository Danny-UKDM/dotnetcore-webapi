using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class DeleteEventsCommand : ICommand
    {
        public IPreparedCommand Prepare(ICommandBuilder builder) =>
            builder
                .WithSql(@"delete from events")
                .Build();
    }
}
