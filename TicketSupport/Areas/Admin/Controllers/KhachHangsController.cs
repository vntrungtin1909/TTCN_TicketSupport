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
using TicketSupport.Library;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    [AuthorizePermissions("KH")]
    [MyAuthenFilter]
    public class KhachHangsController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
		// GET: Admin/KhachHangs
		[AuthorizeRoles("KH-R")]
		public ActionResult Index()
        {
            return View(db.tblkhachhangs.ToList());
        }
		[AuthorizeRoles("KH-R")]
		// GET: Admin/KhachHangs/Details/5
		public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblkhachhang tblkhachhang = db.tblkhachhangs.Find(id);
            if (tblkhachhang == null)
            {
                return HttpNotFound();
            }
            return View(tblkhachhang);
        }
		[AuthorizeRoles("KH-C")]
		// GET: Admin/KhachHangs/Create
		public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		[AuthorizeRoles("KH-C")]
		public ActionResult Create(tblkhachhang tblkhachhang)
        {

            if (string.IsNullOrEmpty(tblkhachhang.mat_khau))
            {
                ModelState.AddModelError("mat_khau", "Mật khẩu không được để trống");
            }
            if (string.IsNullOrEmpty(tblkhachhang.email))
            {
                ModelState.AddModelError("email", "Email không được để trống");
            }
            if (string.IsNullOrEmpty(tblkhachhang.ho_ten_khach_hang))
            {
                ModelState.AddModelError("ho_ten_khach_hang", "Họ tên khách hàng không được để trống");
            }
            //if (string.IsNullOrEmpty(tblkhachhang.ma_khach_hang))
            //{
            //    ModelState.AddModelError("ma_khach_hang", "Mã khách hàng không được để trống");
            //}
            if (string.IsNullOrEmpty(tblkhachhang.so_dien_thoai))
            {
                ModelState.AddModelError("so_dien_thoai", "Số điện thoại không được để trống");
            }
            bool emailExists = db.tblkhachhangs.Any(kh => kh.email == tblkhachhang.email);
            if (emailExists)
            {
                ModelState.AddModelError("email", "Email đã tồn tại trong hệ thống");
            }
            if (ModelState.IsValid)
            {
                tblkhachhang lastCus = db.tblkhachhangs
                                    .OrderByDescending(kh => kh.ma_khach_hang)
                                    .FirstOrDefault();
                if (lastCus == null)
                {
                    tblkhachhang.ma_khach_hang = "KH00001";
                }
                else
                {
                    int num = int.Parse(lastCus.ma_khach_hang.Substring(2));
                    tblkhachhang.ma_khach_hang = "KH" + (num + 1).ToString("D5");
                }
                string plainPass = tblkhachhang.mat_khau;
                tblkhachhang.mat_khau = MaHoa.HashPassword(plainPass);
                tblkhachhang.trang_thai = true;
                tblkhachhang.ngay_tao = DateTime.Now;
                db.tblkhachhangs.Add(tblkhachhang);
                db.SaveChanges();

                string subject = "Thông báo tạo tài khoản thành công";
                string body = $"<p>Chào {tblkhachhang.ho_ten_khach_hang}</p>" +
                          $"<p>Email đăng nhập: <b>{tblkhachhang.email}</b></p>" +
                          $"<p>Mật khẩu đăng nhập: <b>{plainPass}</b></p>" +
                          $"<p>Xin cảm ơn</p>";

                EmailHelper.SendEmail(tblkhachhang.email, subject, body);
                return RedirectToAction("Index");
            }

            return View(tblkhachhang);
        }
		[AuthorizeRoles("KH-E")]
		// GET: Admin/KhachHangs/Edit/5
		public ActionResult Edit(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Mã khách hàng không tồn tại");
                return RedirectToAction("Index");
            }
            tblkhachhang tblkhachhang = db.tblkhachhangs.Find(id);
            if (tblkhachhang == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Khách hàng không tồn tại");
                return RedirectToAction("Index");
            }
            tblkhachhang.mat_khau = "";
            return View(tblkhachhang);
        }
		[AuthorizeRoles("KH-E")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblkhachhang tblkhachhang)
        {
            //if(string.IsNullOrEmpty(tblkhachhang.mat_khau) || tblkhachhang.mat_khau.Trim() == "")
            //{
            //    ModelState.AddModelError("mat_khau", "Mật khẩu không được để trống");
            //}
            if (string.IsNullOrEmpty(tblkhachhang.email) || tblkhachhang.email.Trim() == "")
            {
                ModelState.AddModelError("email", "Email không được để trống");
            }
            if (string.IsNullOrEmpty(tblkhachhang.ho_ten_khach_hang) || tblkhachhang.ho_ten_khach_hang.Trim() == "")
            {
                ModelState.AddModelError("ho_ten_khach_hang", "Họ tên khách hàng không được để trống");
            }
            if (string.IsNullOrEmpty(tblkhachhang.ma_khach_hang) || tblkhachhang.ma_khach_hang.Trim() == "")
            {
                ModelState.AddModelError("ma_khach_hang", "Mã khách hàng không được để trống");
            }
            if (string.IsNullOrEmpty(tblkhachhang.so_dien_thoai) || tblkhachhang.so_dien_thoai.Trim() == "")
            {
                ModelState.AddModelError("so_dien_thoai", "Số điện thoại không được để trống");
            }

            if (ModelState.IsValid)
            {
                tblkhachhang cus = db.tblkhachhangs.AsNoTracking().FirstOrDefault(u => u.ma_khach_hang == tblkhachhang.ma_khach_hang);

                if (cus == null)
                {
                    TempData["message"] = new XMessage("danger", "Khách hàng không tồn tại");
                    return RedirectToAction("Index");
                }
                tblkhachhang.mat_khau += " ";
                if (tblkhachhang.mat_khau.Trim() == "")
                {
                    tblkhachhang.mat_khau = cus.mat_khau;
                }
                else
                {
                    tblkhachhang.mat_khau = MaHoa.HashPassword(tblkhachhang.mat_khau);
                }

                if (tblkhachhang.email == cus.email)
                {
                    tblkhachhang.email = cus.email;
                }
                else if (tblkhachhang.email != cus.email &&  db.tblkhachhangs.Any(u => u.email == tblkhachhang.email))
                {
                    TempData["message"] = new XMessage("danger", "Email đã tồn tại");
                    return View();
                }
                tblkhachhang.trang_thai = Request.Form["trang_thai"] == "true" ? true : false;
                tblkhachhang.cap_nhat = DateTime.Now;
                db.Entry(tblkhachhang).State = EntityState.Modified;
                db.SaveChanges();
                //thogn bao thanh cong
                TempData["message"] = new XMessage("success", "cập nhật mẫu tin thành công");
                return RedirectToAction("Index");
            }
            return View(tblkhachhang);
        }
		[AuthorizeRoles("KH-D")]
		// GET: Admin/KhachHangs/Delete/5
		public ActionResult Delete(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            tblkhachhang tblkhachhang = db.tblkhachhangs.Find(id);
            if (tblkhachhang == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(tblkhachhang);
        }
		[AuthorizeRoles("KH-D")]
		// POST: Admin/KhachHangs/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblkhachhang tblkhachhang = db.tblkhachhangs.Find(id);
            db.tblkhachhangs.Remove(tblkhachhang);
            db.SaveChanges();
            TempData["message"] = new XMessage("success", "xóa mẫu tin thành  công");
            return RedirectToAction("Index");
        }
		[AuthorizeRoles("KH-E", "KH-D", "KH-C")]
		public ActionResult Status(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }
            //truy van id
            tblkhachhang user = db.tblkhachhangs.Find(id);
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
