using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TicketSupport.Models;
using TicketSupport.ViewModels;
using CaptchaMvc.HtmlHelpers;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using BCrypt.Net;
using hbehr.recaptcha;
using System.Web.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

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
		public List<string> GetUserPermisions(string userId)
		{
			var permission = db.tblnguoidungs
				.Where(nd => nd.ma_nguoi_dung == userId)
				.SelectMany(nd => nd.tblphongbans)
				.SelectMany(pb => pb.tblquyens)
				.Select(cn => cn.tblchucnang).Select(mcn=>mcn.ma_chuc_nang)
				.Distinct()
				.ToList();

			return permission;
		}
		[HttpPost]
        public ActionResult Login(string email, string mat_khau, string captcha)
        {

            
            tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.email == email || u.ten_dang_nhap == email || u.mat_khau == mat_khau);

            if (user == null )
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
			var userPermissions = GetUserPermisions(user.ma_nguoi_dung);


			Session["UserId"] = user.ma_nguoi_dung;
            Session["Username"] = user.ho_ten_nguoi_dung;
            Session["UserRoles"] = userRoles; // Lưu danh sách quyền vào Session
            Session["UserPermissions"] = userPermissions; // Lưu danh sách chức năng vào Session
            //return RedirectToAction("Index", "Home");
			return RedirectToAction("Index", "Dashboard");


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
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                string resetLink = Url.Action("ResetPassword", "Login", new { token = user.token }, Request.Url.Scheme);
                string subject = "Yêu cầu đặt lại mật khẩu";
                string body = $"<p>Chào {user.ho_ten_nguoi_dung},</p>" +
                          $"<p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào liên kết bên dưới để đặt lại mật khẩu:</p>" +
                          $"<a href='{resetLink}'>Đặt lại mật khẩu</a>" +
                          $"<p>Liên kết sẽ hết hạn sau 1 giờ.</p>";

            EmailHelper.SendEmail(user.email, subject, body);
            ViewBag.Success = "Email";
                return View();  
            
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            
            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.token == token && u.token_expire > DateTime.Now);

                if (user == null)
                {
                    ViewBag.Message = "Token không hợp lệ hoặc hết hạn";
                    return View();
                }
                ViewBag.first = TempData["first"];
                ViewBag.Token = token;
                return View();
            
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, ChangePassVM model)
        {

            
                tblnguoidung user = db.tblnguoidungs.FirstOrDefault(u => u.token == token);

                if (user == null || user.token_expire <= DateTime.Now)
                {
                    ViewBag.Message = "Token";
                    ViewBag.Token = token;
                    return View(model);
                }
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                     ViewBag.Message = "Pass";
                    ViewBag.Token = token;
                    return View(model);
                }
                user.mat_khau = MaHoa.HashPassword(model.NewPassword);
                user.token = null;
                user.token_expire = null;
                user.ngay_kich_hoat = DateTime.Now;
                user.cap_nhat = DateTime.Now;
 //               db.tblnguoidungs.Attach(user);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công";
                return RedirectToAction("Index");
            
        }

        public ActionResult LogOut()
        {
            Session["UserId"] = null;
            Session["Username"] = null;
            Session["UserRoles"] = null;
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        //[HttpGet]
        //public ActionResult GenerateCaptcha()
        //{
        //    string captchaCode = GenerateRandomCode(6); // Số ký tự của mã CAPTCHA
        //    Session["Captcha"] = captchaCode; // Lưu mã CAPTCHA vào Session

        //    using (var bitmap = new Bitmap(200, 50))
        //    using (var graphics = Graphics.FromImage(bitmap))
        //    {
        //        // Tạo màu nền và màu văn bản
        //        graphics.Clear(Color.White);
        //        using (var font = new Font("Arial", 20, FontStyle.Bold))
        //        {
        //            using (var brush = new SolidBrush(Color.Black))
        //            {
        //                graphics.DrawString(captchaCode, font, brush, new PointF(10, 10));
        //            }
        //        }

        //        // Thêm các đường nét ngẫu nhiên để làm khó nhận diện
        //        var pen = new Pen(Color.Gray, 2);
        //        for (int i = 0; i < 5; i++)
        //        {
        //            graphics.DrawLine(pen, new PointF(i * 40, 0), new PointF(i * 40, 50));
        //        }

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            bitmap.Save(memoryStream, ImageFormat.Png);
        //            return File(memoryStream.ToArray(), "image/png");
        //        }
        //    }
        //}

        //private string GenerateRandomCode(int length)
        //{
        //    var random = new Random();
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        //}
            private const string RecaptchaSecretKey = "YOUR_SECRET_KEY"; // Thay bằng Secret Key của bạn
            private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

            [HttpPost]
            public async Task<JsonResult> VerifyCaptcha(string captcha)
            {
                try
                {
                    if (string.IsNullOrEmpty(captcha))
                    {
                        return Json(new { success = false, message = "Captcha không được để trống." });
                    }

                    using (var client = new HttpClient())
                    {
                        // Chuẩn bị dữ liệu gửi tới API reCAPTCHA
                        var values = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("secret", RecaptchaSecretKey),
                        new KeyValuePair<string, string>("response", captcha)
                    });

                        // Gửi yêu cầu đến API của Google
                        var response = await client.PostAsync(RecaptchaVerifyUrl, values);
                        if (!response.IsSuccessStatusCode)
                        {
                            return Json(new { success = false, message = "Không thể xác thực reCAPTCHA." });
                        }

                        var responseContent = await response.Content.ReadAsStringAsync();
                        var captchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(responseContent);

                        // Kiểm tra kết quả từ Google
                        if (captchaResult.Success)
                        {
                            return Json(new { success = true, message = "Captcha hợp lệ." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Captcha không hợp lệ." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
                    return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
                }
            }

            // Lớp để ánh xạ dữ liệu từ phản hồi của Google reCAPTCHA
            public class RecaptchaResponse
            {
                [JsonProperty("success")]
                public bool Success { get; set; }

                [JsonProperty("challenge_ts")]
                public DateTime ChallengeTimestamp { get; set; }

                [JsonProperty("hostname")]
                public string Hostname { get; set; }

                [JsonProperty("error-codes")]
                public string[] ErrorCodes { get; set; }
            }
        }
    }

