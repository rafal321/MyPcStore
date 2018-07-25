using MyPcStore.Models.Data;
using MyPcStore.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPcStore.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //list of page view models
            //Declare list of Page View Models
            List<PageVM> pageslist;

            //initialize list - using statement for connection closing
            using (Db db = new Db())
            {                                                        //need empty constructor here Alt+f12
                pageslist = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //return view with list
            return View(pageslist);
        }

        //GET: Admin/Pages/AddPage
        public ActionResult AddPage()
        {
            return View();
        }
    }
}