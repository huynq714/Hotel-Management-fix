using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Hotel_Management.Models
{
    public class StaffViewModel
    {
        public int? AccountID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string CCCD { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; } = "Staff";
        public string Status { get; set; } = "Active";
    }
}