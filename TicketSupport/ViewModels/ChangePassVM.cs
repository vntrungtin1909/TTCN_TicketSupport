using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSupport.ViewModels
{
    public class ChangePassVM
    {

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Mật khẩu không khớp")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is required")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm New Password")]

        public string ConfirmNewPassword { get; set; }
    }
}