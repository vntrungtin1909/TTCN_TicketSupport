using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Models;
using TicketSupport.ViewModels;

namespace TicketSupport.Controllers
{
    public class UserLoginController : Controller
    {
        Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        // GET: UserLogin
        [HttpGet]
        public ActionResult Index()
        {
            return View(new Login());
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View(new Login());
        }

        public List<string>
    GetUserRoles(string userId)
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
        public ActionResult Login(Login model, string captcha)
        {
            if (ModelState.IsValid)
            {
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.email == model.Email || u.ten_dang_nhap == model.Email || u.mat_khau == model.Password);

                if (user == null)
                {
                    ViewBag.User = "Tài khoản không hợp lệ";
                    return View("Index");
                }
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.mat_khau))
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
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View();
            }

            var user = db.tblnguoidungs.FirstOrDefault(u => u.email == email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại trong hệ thống.";
                return View();
            }

            // Tạo token đặt lại mật khẩu
            user.token = Guid.NewGuid().ToString();
            user.token_expire = DateTime.Now.AddHours(1);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // Gửi email chứa liên kết đặt lại mật khẩu
            string resetLink = Url.Action("ResetPassword", "UserLogin", new { token = user.token }, Request.Url.Scheme);
            string subject = "Yêu cầu đặt lại mật khẩu";
            string body = $"Chào {user.ho_ten_nguoi_dung},<br />Vui lòng nhấn vào liên kết dưới đây để đặt lại mật khẩu của bạn:<br /><a href='{resetLink}'>Đặt lại mật khẩu</a><br />Liên kết sẽ hết hạn sau 1 giờ.";

            // Giả sử có hàm gửi email (SendEmail)
            SendEmail(user.email, subject, body);

            ViewBag.Success = "Email khôi phục mật khẩu đã được gửi. Vui lòng kiểm tra hộp thư.";
            return View();
        }

        // Hàm giả định để gửi email
        private void SendEmail(string toEmail, string subject, string body)
        {
            // Thêm logic gửi email tại đây (SMTP, SendGrid, v.v.)
            Console.WriteLine($"Sending email to {toEmail} with subject {subject}");
        }

        //public ActionResult Register()
        //{
        //    return View();
        //}

        //// POST: Register
        //[HttpPost]
        //public ActionResult Register(tblkhachhang models)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Error = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
        //        return View("Register", models);
        //    }

        //    // Kiểm tra mật khẩu và xác nhận mật khẩu có trùng khớp không
        //    if (models.mat_khau != models.)
        //    {
        //        ViewBag.PasswordMismatch = "Mật khẩu và xác nhận mật khẩu không khớp.";
        //        return View("Register", models);
        //    }

        //    // Kiểm tra nếu email đã tồn tại trong hệ thống
        //    if (db.tblkhachhangs.Any(u => u.email == models.email))
        //    {
        //        ViewBag.Error = "Email đã tồn tại trong hệ thống.";
        //        return View("Register", models);
        //    }

        //    // Mã hóa mật khẩu
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(models.mat_khau);

        //    // Tạo người dùng mới
        //    var newUser = new tblkhachhang
        //    {
        //        ma_khach_hang = Guid.NewGuid().ToString(),
        //        ho_ten_khach_hang = models.ho_ten_khach_hang,
        //        email = models.email,
        //        so_dien_thoai = models.so_dien_thoai,
        //        ten_cong_ty = models.ten_cong_ty,
        //        ma_so_thue = models.ma_so_thue,
        //        phan_mem = models.phan_mem,
        //        mat_khau = hashedPassword,
        //        ngay_kich_hoat = DateTime.Now,
        //        token = null,
        //        token_expire = null
        //    };

        //    db.tblkhachhangs.Add(newUser);
        //    db.SaveChanges();

        //    ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
        //    return RedirectToAction("Login");
        //}

    }
}
