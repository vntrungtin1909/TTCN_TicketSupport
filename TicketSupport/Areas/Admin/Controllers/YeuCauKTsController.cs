using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Library;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    public class YeuCauKTsController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        // GET: Admin/YeuCauKTs
        public ActionResult Index()
        {
            
            return View(db.tblyeucauhotrokythuats.ToList());
        }

        // GET: Admin/YeuCauKTs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                return HttpNotFound();
            }
            var tenPhongBan = db.tblphongbans.Where(p => p.ma_phong_ban == tblyeucauhotrokythuat.ma_phong_ban).Select(p => p.ten_phong_ban).FirstOrDefault();
            var tenKhachHang = db.tblkhachhangs.Where(p => p.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang).Select(p => p.ho_ten_khach_hang).FirstOrDefault();
            ViewBag.tenphong = tenPhongBan ?? "Không tồn tại";
            ViewBag.tenkhach = tenKhachHang ?? "Không tồn tại";
            return View(tblyeucauhotrokythuat);
        }

        // GET: Admin/YeuCauKTs/Create
        public ActionResult Create()
        {
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban");
            return View();
        }

        // POST: Admin/YeuCauKTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblyeucauhotrokythuat tblyeucauhotrokythuat)
        {
            if (ModelState.IsValid)
            {
                tblyeucauhotrokythuat lastReq = db.tblyeucauhotrokythuats
                                    .OrderByDescending(kh => kh.ma_yeu_cau)
                                    .FirstOrDefault();
                if (lastReq == null)
                {
                    tblyeucauhotrokythuat.ma_yeu_cau = "YC00001";
                }
                else
                {
                    int num = int.Parse(lastReq.ma_yeu_cau.Substring(2));
                    tblyeucauhotrokythuat.ma_yeu_cau = "YC" + (num + 1).ToString("D5");
                }
                tblyeucauhotrokythuat.ngay_tiep_nhan = DateTime.Now;
                if (tblyeucauhotrokythuat.trang_thai_xu_ly == "0")
                {
                    tblyeucauhotrokythuat.ngay_xu_ly = null;
                }
                else
                {
                    tblyeucauhotrokythuat.ngay_xu_ly = DateTime.Now;
                }
                if (tblyeucauhotrokythuat.ma_khach_hang.Trim() != "")
                {
                    if (!db.tblkhachhangs.Any(u => u.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang))
                    {
                        TempData["message"] = new XMessage("danger", "Không tìm thấy mã khách hàng");
                        ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
                        return View(tblyeucauhotrokythuat);
                    }
                }
                    db.tblyeucauhotrokythuats.Add(tblyeucauhotrokythuat);
                    db.SaveChanges();
                    return RedirectToAction("Index");
            }
            
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }

        // GET: Admin/YeuCauKTs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                return HttpNotFound();
            }
           
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }

        // POST: Admin/YeuCauKTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( tblyeucauhotrokythuat tblyeucauhotrokythuat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblyeucauhotrokythuat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }

        // GET: Admin/YeuCauKTs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                return HttpNotFound();
            }
            return View(tblyeucauhotrokythuat);
        }

        // POST: Admin/YeuCauKTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            db.tblyeucauhotrokythuats.Remove(tblyeucauhotrokythuat);
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
