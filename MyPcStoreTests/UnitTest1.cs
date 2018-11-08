using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPcStore.Areas.Admin.Controllers;
using MyPcStore.Models.ViewModels.Pages;


namespace MyPcStoreTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AdminIndex()
        {
            DashboardController controller = new DashboardController();

            var result = controller.Index() as ViewResult;

            //Asert

            Assert.IsNotNull(result);

        }



        //=====================================================
        [TestMethod]
        public void CanAddPage()
        {
            //Arrange
            PagesController controller = new PagesController(new TestDb());

            PageVM pageVM = new PageVM
            {
                Title = "TTTT",
                Body = "bb",
                HasSidebar = true,
                Id = 1,

            };

            //Act
            var result = controller.AddPage(pageVM) as RedirectToRouteResult;

            //var result = controller.AddPage(pageVM);

            //Result

            Assert.AreEqual("AddPage", result.RouteValues["action"]);
            //Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CanNotAddPage()
        {
            //Arrange
            PagesController controller = new PagesController(new TestDb());
            controller.ModelState.AddModelError("", "Error");

            PageVM pageVM = new PageVM
            {
                Title = "TTTT",
                Body = "bb",
                HasSidebar = true,
                Id = 1,

            };

            //Act
            var result = controller.AddPage(pageVM);

            //var result = controller.AddPage(pageVM);

            //Result

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


    }
}
