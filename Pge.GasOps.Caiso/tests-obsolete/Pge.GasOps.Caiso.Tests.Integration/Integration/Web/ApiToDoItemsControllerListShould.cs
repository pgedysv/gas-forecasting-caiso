using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pge.GasOps.Caiso.Core.Entities;
using Xunit;

namespace Pge.GasOps.Caiso.Tests.Integration.Integration.Web
{

    public class ApiToDoItemsControllerList : BaseWebTest
    {
        // TODO: Fix this integration test.
        [Fact(Skip = "This integration test is having issues running due to a System.Runtime verion mismatch.")]
        public async Task ReturnsTwoItems()
        {
            var response = await Client.GetAsync("/api/todoitems");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ToDoItem>>(stringResponse).ToList();

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.Count(a => a.Title == "Test Item 1"));
            Assert.Equal(1, result.Count(a => a.Title == "Test Item 2"));
        }
    }
}