using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TicketSupport.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Họ tên khách hàng là bắt buộc")]
        [Display(Name = "Họ tên khách hàng")]
        public string HoTenKhachHang { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Tên công ty là bắt buộc")]
        [Display(Name = "Tên công ty")]
        public string TenCongTy { get; set; }

        [Required(ErrorMessage = "Mã số thuế là bắt buộc")]
        [Display(Name = "Mã số thuế")]
        public string MaSoThue { get; set; }

        [Required(ErrorMessage = "Phần mềm là bắt buộc")]
        [Display(Name = "Phần mềm")]
        public string PhanMem { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("XacNhanMatKhau", ErrorMessage = "Mật khẩu không khớp")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; }
    }
}