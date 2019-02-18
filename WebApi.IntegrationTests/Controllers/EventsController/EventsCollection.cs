using WebApi.IntegrationTests.Controllers.EventsController.Put;
using Xunit;

namespace WebApi.IntegrationTests.Controllers.EventsController
{
    [CollectionDefinition(nameof(EventsCollection))]
    public class EventsCollection : ICollectionFixture<ApiWebApplicationFactory>
    {
    }
}