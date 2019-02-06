using Badger.Data;

namespace WebApi.IntegrationTests.Data
{
    public class QueryForAllRowsCount : IQuery<long>
    {
        public IPreparedQuery<long> Prepare(IQueryBuilder queryBuilder)
        {
            return queryBuilder.WithSql("select count(*) from events")
                               .WithScalar<long>()
                               .Build();
        }
    }
}
