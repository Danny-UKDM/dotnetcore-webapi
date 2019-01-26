using Npgsql;
using Xunit;

namespace WebApi.Tests.Tools.DbFactory
{
    public class GivenADbFactory
    {
        [Fact]
        public void ThenTheDatabaseIsCreated()
        {
            var factory = new WebApi.Tools.DbFactory(NpgsqlFactory.Instance);
            factory.InitDatabase();
            factory.Dispose();
        }
    }
}
