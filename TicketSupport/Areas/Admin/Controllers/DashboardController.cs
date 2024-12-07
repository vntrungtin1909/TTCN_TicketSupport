using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Filters;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Views
{
	[MyAuthenFilter]
	public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
			return View();
        }
		public ActionResult Unauthorized()
		{
			return View();
		}

	}
}
