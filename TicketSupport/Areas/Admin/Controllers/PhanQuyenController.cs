using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using TicketSupport.Areas.Admin.Authorization;
using TicketSupport.Filters;
using TicketSupport.Library;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    [AuthorizeRoles("ADMIN")]
    [MyAuthenFilter]
    public class PhanQuyenController : Controller
    {
        Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        public ActionResult Index()
        {
            return View(db.tblphongbans.ToList());
        }

        // GET: Details
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }
            return View(tblphongban);
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen");
            return View();
        }

        [HttpPost]
        public ActionResult Create(tblphongban tblphongban, List<string> SelectedQuyen)
        {
            if (ModelState.IsValid)
            {
                tblphongban.tblquyens = db.tblquyens.Where(q => SelectedQuyen.Contains(q.ma_quyen)).ToList();
                tblphongban.trang_thai = true; // Mặc định kích hoạt
                db.tblphongbans.Add(tblphongban);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", SelectedQuyen);
            return View(tblphongban);
        }

        // GET: Edit
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", tblphongban.tblquyens.Select(q => q.ma_quyen));
            return View(tblphongban);
        }

        [HttpPost]
        public ActionResult Edit(tblphongban tblphongban, List<string> SelectedQuyen)
        {
            if (ModelState.IsValid)
            {
                var phongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == tblphongban.ma_phong_ban);
                if (phongban == null)
                {
                    return HttpNotFound();
                }

                phongban.ten_phong_ban = tblphongban.ten_phong_ban;
                phongban.trang_thai = tblphongban.trang_thai;
                phongban.tblquyens.Clear();

                if (SelectedQuyen != null)
                {
                    phongban.tblquyens = db.tblquyens.Where(q => SelectedQuyen.Contains(q.ma_quyen)).ToList();
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", SelectedQuyen);
            return View(tblphongban);
        }


        // GET: Delete
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }
            return View(tblphongban);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }

            db.tblphongbans.Remove(tblphongban);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Status
        public ActionResult Status(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }

            var tblphongban = db.tblphongbans.Find(id);
            if (tblphongban == null)
            {
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }

            tblphongban.trang_thai = !tblphongban.trang_thai; // Đổi trạng thái
            db.Entry(tblphongban).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["message"] = new XMessage("success", "cap nhat trang thai thanh cong");
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
