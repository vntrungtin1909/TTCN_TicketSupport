using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Areas.Admin;
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
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                tblkhachhang user = db.tblkhachhangs.FirstOrDefault(u => u.email == model.Email || u.mat_khau == model.Password);

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

                //if (user.ngay_kich_hoat == null)
                //{
                //    user.token = Guid.NewGuid().ToString();
                //    db.Entry(user).State = EntityState.Modified;
                //    db.SaveChanges();
                //    TempData["first"] = "Đổi mật khẩu cho lần đầu đăng nhập";
                //    string resetLink = Url.Action("ResetPassword", "Login", new { token = user.token }, Request.Url.Scheme);
                //    return Redirect(resetLink);
                //}

                var userRoles = GetUserRoles(user.ma_khach_hang);

                Session["UserId"] = user.ma_khach_hang;
                Session["Username"] = user.ho_ten_khach_hang;
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
        

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(ViewModels.Register register)
        {
            if (string.IsNullOrEmpty(register.MatKhau))
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống");
            }
            if (string.IsNullOrEmpty(register.Email))
            {
                ModelState.AddModelError("Email", "Email không được để trống");
            }
            if (string.IsNullOrEmpty(register.HoTenKhachHang))
            {
                ModelState.AddModelError("HoTenKhachHang", "Họ tên khách hàng không được để trống");
            }
            if (string.IsNullOrEmpty(register.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại không được để trống");
            }
            if (string.IsNullOrEmpty(register.TenCongTy))
            {
                ModelState.AddModelError("TenCongTy", "Tên công ty không được để trống");
            }
            if (string.IsNullOrEmpty(register.MaSoThue))
            {
                ModelState.AddModelError("MaSoThue", "Mã số thuế không được để trống");
            }

            // Kiểm tra email đã tồn tại
            bool emailExists = db.tblkhachhangs.Any(kh => kh.email == register.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống");
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (register.MatKhau != register.XacNhanMatKhau)
            {
                ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu không khớp");
            }

            if (ModelState.IsValid)
            {
                tblkhachhang newCustomer = new tblkhachhang
                {
                    ho_ten_khach_hang = register.HoTenKhachHang,
                    email = register.Email,
                    so_dien_thoai = register.SoDienThoai,
                    ten_cong_ty = register.TenCongTy,
                    ma_so_thue = register.MaSoThue,
                    phan_mem = register.PhanMem,
                    trang_thai = true,
                    ngay_tao = DateTime.Now
                };

                // Tạo mã khách hàng tự động
                tblkhachhang lastCus = db.tblkhachhangs.OrderByDescending(kh => kh.ma_khach_hang).FirstOrDefault();
                if (lastCus == null)
                {
                    newCustomer.ma_khach_hang = "KH00001";
                }
                else
                {
                    int num = int.Parse(lastCus.ma_khach_hang.Substring(2));
                    newCustomer.ma_khach_hang = "KH" + (num + 1).ToString("D5");
                }

                // Hash mật khẩu
                string plainPass = register.MatKhau;
                newCustomer.mat_khau = MaHoa.HashPassword(plainPass);

                // Thêm vào cơ sở dữ liệu
                db.tblkhachhangs.Add(newCustomer);
                db.SaveChanges();

                // Gửi email xác nhận
                string subject = "Thông báo tạo tài khoản thành công";
                string body = $"<p>Chào {newCustomer.ho_ten_khach_hang}</p>" +
                              $"<p>Email đăng nhập: <b>{newCustomer.email}</b></p>" +
                              $"<p>Mật khẩu đăng nhập: <b>{plainPass}</b></p>" +
                              $"<p>Xin cảm ơn</p>";

                EmailHelper.SendEmail(newCustomer.email, subject, body);

                return RedirectToAction("Login");
            }

            return View(register);
        }
        public ActionResult CustomerInfo()
        {
            // Logic để lấy thông tin khách hàng từ session hoặc cơ sở dữ liệu
            string userId = Session["UserId"].ToString();
            var customer = db.tblkhachhangs.FirstOrDefault(c => c.ma_khach_hang == userId);
            return View(customer);  // Trả về view với thông tin khách hàng
        }

        // Action hiển thị danh sách yêu cầu của khách hàng
        public ActionResult RequestList()
        {
            string userId = Session["UserId"].ToString();
            var requests = db.tblyeucauhotrokythuats.Where(r => r.ma_khach_hang == userId).ToList();
            return View(requests);  // Trả về view với danh sách yêu cầu
        }
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Home"); // Điều hướng đến trang chủ hoặc trang đăng nhập
        }
    }
}
