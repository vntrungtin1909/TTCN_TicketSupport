using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Models;

namespace TicketSupport.Controllers
{
    public class UserLoginController : Controller
    {
        Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
        // GET: UserLogin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public ActionResult Login()
        {
            return View();
        }
        public List<string> GetUserRoles(string userId)
        {
            var roles = db.tblnguoidungs
                .Where(nd => nd.ma_nguoi_dung == userId)
                .SelectMany(nd => nd.tblphongbans)
                .SelectMany(pb => pb.tblquyens)
                .Select(q => q.ma_quyen)
                .Distinct()
                .ToList();

            return roles;
        }
        [HttpPost]
        public ActionResult Login(string email, string mat_khau, string captcha)
        {


            tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.email == email || u.ten_dang_nhap == email || u.mat_khau == mat_khau);

            if (user == null)
            {
                ViewBag.User = "Tài khoản không hợp lệ";
                return View("Index");
            }
            if (!BCrypt.Net.BCrypt.Verify(mat_khau, user.mat_khau))
            {
                ViewBag.Pass = "Tài khoản và mật khẩu không khớp";
                return View("Index");
            }

            if (user.ngay_kich_hoat == null)
            {
                user.token = Guid.NewGuid().ToString();
                user.token_expire = DateTime.Now.AddHours(1);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["first"] = "Đổi mật khẩu cho lần đầu đăng nhập";
                string resetLink = Url.Action("ResetPassword", "Login", new { token = user.token }, Request.Url.Scheme);
                return Redirect(resetLink);
            }
            var userRoles = GetUserRoles(user.ma_nguoi_dung);


            Session["UserId"] = user.ma_nguoi_dung;
            Session["Username"] = user.ho_ten_nguoi_dung;
            Session["UserRoles"] = userRoles; // Lưu danh sách quyền vào Session
                                              //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "Dashboard");


        }
    }
}