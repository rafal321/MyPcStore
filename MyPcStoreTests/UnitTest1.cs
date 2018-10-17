using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPcStore.Areas.Admin.Controllers;

namespace MyPcStoreTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PagesController controller = new PagesController();

            var result = controller.AddPage() as ViewResult;

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState.AddModelError,
                "This title and/or slug already exists.");            
        }
    }
}
