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
using TicketSupport.Areas.Admin.Authorization;

namespace TicketSupport.Areas.Admin.Controllers
{
	[AuthorizePermissions("NS")]
	[MyAuthenFilter]
    public class tblnguoidungsController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
		[AuthorizeRoles("NS-R")]
		// GET: Admin/tblnguoidungs
		public ActionResult Index()
        {
            return View(db.tblnguoidungs.ToList());
        }
		[AuthorizeRoles("NS-R")]
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
		[AuthorizeRoles("NS-C")]
		// GET: Admin/tblnguoidungs/Create
		public ActionResult Create()
        {
            ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban");
            //ViewBag.PhongBans = db.tblphongbans.ToList();
            return View();
        }
		[AuthorizeRoles("NS-C")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblnguoidung tblnguoidung, List<String> chon)
        {
            if (string.IsNullOrEmpty(tblnguoidung.ten_dang_nhap))
            {
                ModelState.AddModelError("ten_dang_nhap", "Tên đăng nhập không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.mat_khau))
            {
                ModelState.AddModelError("mat_khau", "Mật khẩu không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.email))
            {
                ModelState.AddModelError("email", "Email không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.ho_ten_nguoi_dung))
            {
                ModelState.AddModelError("ho_ten_nguoi_dung", "Họ tên người dùng không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.so_dien_thoai))
            {
                ModelState.AddModelError("so_dien_thoai", "Số điện thoại không được để trống");
            }
            bool emailExists = db.tblnguoidungs.Any(kh => kh.email == tblnguoidung.email);
            if (emailExists)
            {
                ModelState.AddModelError("email", "Email đã tồn tại trong hệ thống");
            }
            bool userExists = db.tblnguoidungs.Any(kh => kh.email == tblnguoidung.email);
            if (userExists)
            {
                ModelState.AddModelError("ten_dang_nhap", "Tên đăng nhập đã tồn tại trong hệ thống");
            }
            if (ModelState.IsValid)
            {
                tblnguoidung lastUser = db.tblnguoidungs
                                    .OrderByDescending(kh => kh.ma_nguoi_dung)
                                    .FirstOrDefault();
                if (lastUser == null)
                {
                    tblnguoidung.ma_nguoi_dung = "NV00001";
                }
                else
                {
                    int num = int.Parse(lastUser.ma_nguoi_dung.Substring(2));
                    tblnguoidung.ma_nguoi_dung = "NV" + (num + 1).ToString("D5");
                }
                string plainPass = tblnguoidung.mat_khau;
                tblnguoidung.mat_khau = MaHoa.HashPassword(plainPass);
                tblnguoidung.trang_thai = true;
                tblnguoidung.token_expire = null;
                tblnguoidung.token = null;
                tblnguoidung.ngay_tao = DateTime.Now;
                tblnguoidung.cap_nhat = null;
                if (chon != null)
                {
                    foreach (var id in chon)
                    {
                        var phongBan = db.tblphongbans.Find(id);
                        if (phongBan != null)
                        {
                            tblnguoidung.tblphongbans.Add(phongBan);
                        }
                    }
                }
                db.tblnguoidungs.Add(tblnguoidung);
                db.SaveChanges();

                string subject = "Thông báo tạo tài khoản thành công";
                string body = $"<p>Chào {tblnguoidung.ho_ten_nguoi_dung}</p>" +                        
                          $"<p>Email đăng nhập: <b>{tblnguoidung.email}</b></p>" +
                          $"<p>Tài khoản đăng nhập: <b>{tblnguoidung.ten_dang_nhap}</b></p>" +
                          $"<p>Mật khẩu đăng nhập: <b>{plainPass}</b></p>" +
                          $"<p>Xin cảm ơn</p>";

            EmailHelper.SendEmail(tblnguoidung.email, subject, body);
                TempData["message"] = new XMessage("success", "Thêm người dùng thành công");
                return RedirectToAction("Index");
            }
            ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban");
            return View(tblnguoidung);
        }
		[AuthorizeRoles("NS-E")]
		// GET: Admin/tblnguoidungs/Edit/5
		public ActionResult Edit(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Mã người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            tblnguoidung tblnguoidung = db.tblnguoidungs.Include("tblphongbans").FirstOrDefault(n => n.ma_nguoi_dung == id);
            if (tblnguoidung == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblnguoidung.tblphongbans.Select(q => q.ma_phong_ban));
            tblnguoidung.mat_khau = "";
            return View(tblnguoidung);
        }
		[AuthorizeRoles("NS-E")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblnguoidung tblnguoidung, List<string> chon)
        {

            //if (string.IsNullOrEmpty(tblnguoidung.mat_khau) || tblnguoidung.mat_khau.Trim() == "")
            //{
            //    ModelState.AddModelError("mat_khau", "Mật khẩu không được để trống");
            //}
            if (string.IsNullOrEmpty(tblnguoidung.email) || tblnguoidung.email.Trim() == "")
            {
                ModelState.AddModelError("email", "Email không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.ten_dang_nhap) || tblnguoidung.ten_dang_nhap.Trim() == "")
            {
                ModelState.AddModelError("ten_dang_nhap", "Tên đăng nhập không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.ho_ten_nguoi_dung) || tblnguoidung.ho_ten_nguoi_dung.Trim() == "")
            {
                ModelState.AddModelError("ho_ten_nguoi_dung", "Họ tên người dùng không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.ma_nguoi_dung) || tblnguoidung.ma_nguoi_dung.Trim() == "")
            {
                ModelState.AddModelError("ma_nguoi_dung", "Mã người dùng không được để trống");
            }
            if (string.IsNullOrEmpty(tblnguoidung.so_dien_thoai) || tblnguoidung.so_dien_thoai.Trim() == "")
            {
                ModelState.AddModelError("so_dien_thoai", "Số điện thoại không được để trống");
            }

            if (ModelState.IsValid)
            {
                //tblnguoidung user = db.tblnguoidungs.AsNoTracking().FirstOrDefault(u => u.ma_nguoi_dung == tblnguoidung.ma_nguoi_dung);
                tblnguoidung user = db.tblnguoidungs.Include("tblphongbans").FirstOrDefault(u => u.ma_nguoi_dung == tblnguoidung.ma_nguoi_dung);

                if (user == null)
                {
                    TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                    return RedirectToAction("Index");
                }
                //tblnguoidung.mat_khau += " ";
                //if (tblnguoidung.mat_khau.Trim() == "")
                //{
                //    tblnguoidung.mat_khau = user.mat_khau;
                //}
                //else
                //{
                //    tblnguoidung.mat_khau = MaHoa.HashPassword(tblnguoidung.mat_khau);
                //}
                //if (tblnguoidung.ten_dang_nhap == user.ten_dang_nhap)
                //{
                //    tblnguoidung.ten_dang_nhap = user.ten_dang_nhap;
                //}
                //else if (tblnguoidung.ten_dang_nhap != user.ten_dang_nhap && db.tblnguoidungs.Any(u => u.ten_dang_nhap == tblnguoidung.ten_dang_nhap))
                //{
                //    TempData["message"] = new XMessage("danger", "Tên đăng nhập đã tồn tại");
                //    return View();
                //}
                //if (tblnguoidung.email == user.email)
                //{
                //    tblnguoidung.email = user.email;
                //}
                //else if (tblnguoidung.email != user.email && db.tblnguoidungs.Any(u => u.email == tblnguoidung.email))
                //{
                //    TempData["message"] = new XMessage("danger", "Email đã tồn tại");
                //    return View();
                //}


                //user.tblphongbans.Clear();
                //if (chon != null)
                //{
                //    foreach (var id in chon)
                //    {
                //        var phongBan = db.tblphongbans.Find(id);
                //        if (phongBan != null)
                //        {
                //            tblnguoidung.tblphongbans.Add(phongBan);
                //        }
                //    }
                //}
                //tblnguoidung.trang_thai = Request.Form["trang_thai"] == "true" ? true : false;
                //tblnguoidung.cap_nhat = DateTime.Now;

                tblnguoidung.mat_khau += " ";
                if (!(tblnguoidung.mat_khau.Trim() == ""))
                {
                    user.mat_khau = MaHoa.HashPassword(tblnguoidung.mat_khau.Trim());
                }

                
                if (tblnguoidung.ten_dang_nhap != user.ten_dang_nhap)
                {
                    if (db.tblnguoidungs.Any(u => u.ten_dang_nhap == tblnguoidung.ten_dang_nhap))
                    {
                        TempData["message"] = new XMessage("danger", "Tên đăng nhập đã tồn tại");
                        ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", chon);
                        return View(tblnguoidung);
                    }
                    else
                    {
                        user.ten_dang_nhap = tblnguoidung.ten_dang_nhap;
                    }
                }
               
                if (tblnguoidung.email != user.email)
                {
                    if (db.tblnguoidungs.Any(u => u.email == tblnguoidung.email))
                    {
                        TempData["message"] = new XMessage("danger", "Email đã tồn tại");
                        ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", chon);
                        return View(tblnguoidung);
                    }
                    else
                    {
                        user.email = tblnguoidung.email;
                    }
                }

                user.tblphongbans.Clear();
                if (chon != null)
                {
                    foreach (var id in chon)
                    {
                        var phongBan = db.tblphongbans.Find(id);
                        if (phongBan != null)
                        {
                            user.tblphongbans.Add(phongBan);
                        }
                    }
                }
                user.trang_thai = Request.Form["trang_thai"] == "true" ? true : false;
                user.cap_nhat = DateTime.Now;
                user.ho_ten_nguoi_dung = tblnguoidung.ho_ten_nguoi_dung;
                user.so_dien_thoai = tblnguoidung.so_dien_thoai;

                //db.Entry(tblnguoidung).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //thogn bao thanh cong
                TempData["message"] = new XMessage("success", "cập nhật mẫu tin thành công");
                return RedirectToAction("Index");
            }
            //ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban");
            ViewBag.PhongBans = new MultiSelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", chon);
            return View(tblnguoidung);
        }
        [AuthorizeRoles("NS-D")]
		// GET: Admin/tblnguoidungs/Delete/5
		public ActionResult Delete(string id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            tblnguoidung tblnguoidung = db.tblnguoidungs.Include("tblphongbans").FirstOrDefault(n => n.ma_nguoi_dung == id);
            if (tblnguoidung == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(tblnguoidung);
        }
		[AuthorizeRoles("NS-D")]
		// POST: Admin/tblnguoidungs/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblnguoidung tblnguoidung = db.tblnguoidungs.Include("tblphongbans").FirstOrDefault(n => n.ma_nguoi_dung == id);
            tblnguoidung.tblphongbans.Clear();
            db.tblnguoidungs.Remove(tblnguoidung);
            db.SaveChanges();
            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "xóa mẫu tin thành  công");
            return RedirectToAction("Index");
        }
		[AuthorizeRoles("NS-C", "NS-E", "NS-D")]
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
