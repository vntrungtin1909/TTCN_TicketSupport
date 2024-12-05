using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TicketSupport.Models;
using TicketSupport.ViewModels;

namespace TicketSupport.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
 
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            
            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.email == email);


            if (user == null )
            {
                ViewBag.Message = "Invalid username or password.";
                return View("Index");
            }
            if (user.password != password)
            {
                ViewBag.Message = "Invalid username or password.";
                return View("Index");
            }
            // Lưu thông tin đăng nhập vào Session
            Session["UserId"] = user.ma_nguoi_dung;
                Session["Username"] = user.email;
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "tblnguoidungs");


        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {

            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.email == email);

                if (user == null)
                {
                    ViewBag.Message = "Email not found.";
                    return View();
                }

                // Tạo token reset mật khẩu
                user.ResetToken = Guid.NewGuid().ToString();
                user.TokenExpiry = DateTime.Now.AddHours(1);
                db.SaveChanges();

                // Gửi email (thay bằng phương thức gửi email thật)
                string resetLink = Url.Action("ResetPassword", "Account", new { token = user.ResetToken }, Request.Url.Scheme);
                ViewBag.Message = $"Reset link sent: {resetLink}";
                return View();
            
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            
            
                var user = db.Users.FirstOrDefault(u => u.ResetToken == token && u.TokenExpiry > DateTime.Now);

                if (user == null)
                {
                    ViewBag.Message = "Invalid or expired token.";
                    return View("Error");
                }

                return View();
            
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, string newPassword)
        {

            
                var user = db.Users.FirstOrDefault(u => u.ResetToken == token && u.TokenExpiry > DateTime.Now);

                if (user == null)
                {
                    ViewBag.Message = "Invalid or expired token.";
                    return View("Error");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.ResetToken = null;
                user.TokenExpiry = null;
                db.SaveChanges();
                ViewBag.Message = "Password reset successful!";
                return RedirectToAction("Login");
            
        }


    }
}