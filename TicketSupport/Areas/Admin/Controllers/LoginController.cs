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
                    ViewBag.Message = "Email không tồn tại";
                    return View();
                }

     
                user.token = Guid.NewGuid().ToString();
                user.token_expire = DateTime.Now.AddHours(1);
                db.SaveChanges();

                string resetLink = Url.Action("ResetPassword", "Login", new { token = user.token }, Request.Url.Scheme);
                string subject = "Yêu cầu đặt lại mật khẩu";
                string body = $"<p>Chào {user.ho_nguoi_dung} {user.ten_nguoi_dung},</p>" +
                          $"<p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào liên kết bên dưới để đặt lại mật khẩu:</p>" +
                          $"<a href='{resetLink}'>Đặt lại mật khẩu</a>" +
                          $"<p>Liên kết sẽ hết hạn sau 1 giờ.</p>";

            EmailHelper.SendEmail(user.email, subject, body);
            ViewBag.Message = "Email đặt lại mật khẩu đã được gửi";
                return View();  
            
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            
            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.token == token && u.token_expire > DateTime.Now);

                if (user == null)
                {
                    ViewBag.Message = "Token không hợp lệ hoặc hết hạn";
                    return View("Error");
                }
                ViewBag.Token = token;
                return View();
            
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, ChangePassVM p)
        {

            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.token == token && u.token_expire > DateTime.Now);

                if (user == null)
                {
                    ViewBag.Message = "Token không hợp lệ hoặc hết hạn";
                    return View("Error");
                }
                if (p.NewPassword != p.ConfirmNewPassword)
                {
                ViewBag.Message = "Mật khẩu không khớp";
                return View();
                }
                user.password = p.NewPassword;
                user.token = null;
                user.token_expire = null;
                db.SaveChanges();
                ViewBag.Message = "Đặt lại mẩu khẩu thành công";
                return RedirectToAction("Login");
            
        }


    }
}