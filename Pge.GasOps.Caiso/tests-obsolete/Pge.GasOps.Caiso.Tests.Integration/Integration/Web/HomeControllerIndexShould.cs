using System.Threading.Tasks;
using Xunit;

namespace Pge.GasOps.Caiso.Tests.Integration.Integration.Web
{
    public class HomeControllerIndexShould : BaseWebTest
    {
        // TODO: Fix this integration test.
        [Fact(Skip = "This integration test is having issues running due to a System.Runtime verion mismatch.")]
        public async Task ReturnViewWithCorrectMessage()
        {
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains((string) "CleanArchitecture.Web", (string) stringResponse);

        }
    }
}