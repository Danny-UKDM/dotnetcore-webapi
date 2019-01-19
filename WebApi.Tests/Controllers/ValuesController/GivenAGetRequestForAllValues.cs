using FluentAssertions;
using WebApi.Controllers;
using Xunit;

namespace WebApi.Tests
{
    public class GivenAGetRequestForAllValues
    {
        [Fact]
        public void ThenAllValuesAreReturned()
        {
            var controller = new ValuesController();
            var values = controller.Get().Value;

            values.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.ContainInOrder(new string[] { "value1", "value2" })
                .And.ContainItemsAssignableTo<string>();
        }
    }
}
