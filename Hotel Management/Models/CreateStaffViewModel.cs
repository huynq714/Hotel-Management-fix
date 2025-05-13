using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Hotel_Management.Models
{
    public class CreateStaffViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "123456";

        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "CCCD là bắt buộc.")]
        public string CCCD { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string Phone { get; set; }

        public string Position { get; set; }
    }
}