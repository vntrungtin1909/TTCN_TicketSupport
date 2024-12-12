using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketSupport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Do()
        {
            ViewBag.Message = "Your Do page.";

            return View();
        }
        public ActionResult Portfolio()
        {
            ViewBag.Message = "Your Portfolio page.";

            return View();
        }
    }
}