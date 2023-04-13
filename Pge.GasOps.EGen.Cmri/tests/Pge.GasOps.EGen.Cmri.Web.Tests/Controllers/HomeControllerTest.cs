using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pge.GasOps.EGen.Cmri.Web;
using Pge.GasOps.EGen.Cmri.Web.Controllers;

namespace Pge.GasOps.EGen.Cmri.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
