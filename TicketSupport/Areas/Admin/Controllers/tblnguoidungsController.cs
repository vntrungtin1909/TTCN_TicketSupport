using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Library;
using TicketSupport.Models;
using TicketSupport.Filters;

namespace TicketSupport.Areas.Admin.Controllers
{
    [MyAuthenFilter]
    public class tblnguoidungsController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        // GET: Admin/tblnguoidungs
        public ActionResult Index()
        {
            return View(db.tblnguoidungs.ToList());
        }

        // GET: Admin/tblnguoidungs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblnguoidung tblnguoidung = db.tblnguoidungs.Find(id);
            if (tblnguoidung == null)
            {
                return HttpNotFound();
            }
            return View(tblnguoidung);
        }

        // GET: Admin/tblnguoidungs/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblnguoidung tblnguoidung)
        {
            if (ModelState.IsValid)
            {
                string plainPass = tblnguoidung.mat_khau;
                tblnguoidung.mat_khau = MaHoa.HashPassword(plainPass);
                tblnguoidung.trang_thai = true;
                tblnguoidung.token_expire = null;
                tblnguoidung.token = null;
                tblnguoidung.ngay_tao = DateTime.Now;
                tblnguoidung.cap_nhat = null;
                db.tblnguoidungs.Add(tblnguoidung);
                db.SaveChanges();

                string subject = "Thông báo tạo tài khoản thành công";
                string body = $"<p>Chào {tblnguoidung.ho_ten_nguoi_dung}</p>" +                        
                          $"<p>Email đăng nhập: <b>{tblnguoidung.email}</b></p>" +
                          $"<p>Tài khoản đăng nhập: <b>{tblnguoidung.ten_dang_nhap}</b></p>" +
                          $"<p>Mật khẩu đăng nhập: <b>{plainPass}</b></p>" +
                          $"<p>Xin cảm ơn</p>";

            EmailHelper.SendEmail(tblnguoidung.email, subject, body);
                return RedirectToAction("Index");
            }

            return View(tblnguoidung);
        }

        // GET: Admin/tblnguoidungs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Mã người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            tblnguoidung tblnguoidung = db.tblnguoidungs.Find(id);
            if (tblnguoidung == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            return View(tblnguoidung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblnguoidung tblnguoidung)
        {
            if (ModelState.IsValid)
            {
                tblnguoidung user = db.tblnguoidungs.AsNoTracking().FirstOrDefault(u => u.ma_nguoi_dung == tblnguoidung.ma_nguoi_dung);

                if (user == null)
                {
                    TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                    return RedirectToAction("Index");
                }
                if (user.mat_khau != tblnguoidung.mat_khau)
                {
                    tblnguoidung.mat_khau = MaHoa.HashPassword(tblnguoidung.mat_khau);
                }
                else
                {
                    tblnguoidung.mat_khau = user.mat_khau;
                }
                tblnguoidung.cap_nhat = DateTime.Now;
                db.Entry(tblnguoidung).State = EntityState.Modified;
                db.SaveChanges();
                //thogn bao thanh cong
                TempData["message"] = new XMessage("success", "cập nhật mẫu tin thành công");
                return RedirectToAction("Index");
            }
            return View(tblnguoidung);
        }

        // GET: Admin/tblnguoidungs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            tblnguoidung tblnguoidung = db.tblnguoidungs.Find(id);
            if (tblnguoidung == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(tblnguoidung);
        }

        // POST: Admin/tblnguoidungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblnguoidung tblnguoidung = db.tblnguoidungs.Find(id);
            db.tblnguoidungs.Remove(tblnguoidung);
            db.SaveChanges();
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "xóa mẫu tin thành  công");
            return RedirectToAction("Index");
        }
       
        public ActionResult Status(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }
            //truy van id
            tblnguoidung user = db.tblnguoidungs.Find(id);
            if (user == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }

            //chuyen doi trang thai cua status 
            if (user.trang_thai == true)
            {
                user.trang_thai = false;
            }
            else
            {
                user.trang_thai = true;
            }
            user.cap_nhat = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            //cap nhat trang thai thanh cong
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
