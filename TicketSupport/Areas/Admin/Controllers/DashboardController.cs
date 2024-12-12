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
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
		// GET: Admin/Dashboard
		[HttpGet]		
		
        public ActionResult Index()
        {
			ViewBag.nguoiDung = db.tblnguoidungs.Count().ToString();
			ViewBag.phongBan = db.tblphongbans.Count().ToString();
			ViewBag.quyen = db.tblquyens.Count().ToString();
			ViewBag.khachHang = db.tblkhachhangs.Count().ToString();
			ViewBag.yeucau = db.tblyeucauhotrokythuats.Count().ToString();
			return View();
        }
		public ActionResult Unauthorized()
		{
			return View();
		}

	}
}
