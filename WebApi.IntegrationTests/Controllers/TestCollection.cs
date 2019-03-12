using Xunit;

namespace WebApi.IntegrationTests.Controllers
{
    [CollectionDefinition(nameof(TestCollection))]
    public class TestCollection : ICollectionFixture<ApiWebApplicationFactory>
    {
    }
}
