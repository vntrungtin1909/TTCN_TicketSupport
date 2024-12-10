using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Areas.Admin.Authorization;
using TicketSupport.Filters;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
	[AuthorizeRoles("ADMIN")]
	[MyAuthenFilter]
	public class QuyensController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        // GET: Admin/Quyens
        public ActionResult Index()
        {
            var tblquyens = db.tblquyens.Include(t => t.tblchucnang);
            return View(tblquyens.ToList());
        }

        // GET: Admin/Quyens/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblquyen tblquyen = db.tblquyens.Find(id);
            if (tblquyen == null)
            {
                return HttpNotFound();
            }
            return View(tblquyen);
        }

        // GET: Admin/Quyens/Create
        public ActionResult Create()
        {
            ViewBag.ma_chuc_nang = new SelectList(db.tblchucnangs, "ma_chuc_nang", "ten_chuc_nang");
            return View();
        }

        // POST: Admin/Quyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_quyen,ma_chuc_nang,ten_quyen,ngay_tao,cap_nhat")] tblquyen tblquyen)
        {
            if (ModelState.IsValid)
            {
                db.tblquyens.Add(tblquyen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_chuc_nang = new SelectList(db.tblchucnangs, "ma_chuc_nang", "ten_chuc_nang", tblquyen.ma_chuc_nang);
            return View(tblquyen);
        }

        // GET: Admin/Quyens/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblquyen tblquyen = db.tblquyens.Find(id);
            if (tblquyen == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_chuc_nang = new SelectList(db.tblchucnangs, "ma_chuc_nang", "ten_chuc_nang", tblquyen.ma_chuc_nang);
            return View(tblquyen);
        }

        // POST: Admin/Quyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_quyen,ma_chuc_nang,ten_quyen,ngay_tao,cap_nhat")] tblquyen tblquyen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblquyen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_chuc_nang = new SelectList(db.tblchucnangs, "ma_chuc_nang", "ten_chuc_nang", tblquyen.ma_chuc_nang);
            return View(tblquyen);
        }

        // GET: Admin/Quyens/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblquyen tblquyen = db.tblquyens.Find(id);
            if (tblquyen == null)
            {
                return HttpNotFound();
            }
            return View(tblquyen);
        }

        // POST: Admin/Quyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblquyen tblquyen = db.tblquyens.Find(id);
            db.tblquyens.Remove(tblquyen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
