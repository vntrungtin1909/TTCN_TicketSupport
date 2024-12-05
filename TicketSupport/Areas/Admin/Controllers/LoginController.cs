using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string email, string password)
        {
            using (var db = new Tech_Support_TicketEntities())
            {
                // Kiểm tra tài khoản đã tồn tại
                if (db.tblnguoidungs.Any(u => u.email == email))
                {
                    ViewBag.Message = "Username already exists.";
                    return View();
                }

                // Tạo tài khoản mới
                var user = new tblnguoidung
                {
                    ten_nguoi_dung = username,
                    email = email,
                    password = BCrypt.Net.BCrypt.HashPassword(password) // Hash mật khẩu
                };

                db.tblnguoidungs.Add(user);
                db.SaveChanges();
                ViewBag.Message = "Đăng kí thành công";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new Tech_Support_TicketEntities();
            
                var user = db.tblnguoidungs.FirstOrDefault(u => u.email == email);

                //if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
                //{
                //    ViewBag.Message = "Invalid username or password.";
                //    return View();
                //}
                //if (user == null )
                //{
                //    ViewBag.Message = "Invalid username or password.";
                //    return View("Index");
                //}
                // Lưu thông tin đăng nhập vào Session
                Session["UserId"] = user.ma_nguoi_dung;
                Session["Username"] = user.email;
                return RedirectToAction("Index", "Home");
            
        }

    }
}