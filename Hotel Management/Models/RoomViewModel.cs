using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hotel_Management.Models
{
    public class RoomViewModel
    {
        public int RoomID { get; set; }

        [Required]
        public int BuildingID { get; set; }

        public int NumberOfFloor { get; set; }

        [Required]
        public int FloorID { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeID { get; set; }

        [Required]
        public string Status { get; set; }

        // Thông tin chi tiết từ RoomType
        public string RoomTypeName { get; set; }
        public decimal RoomPrice { get; set; }
        public int MaxAdult { get; set; }
        public int MaxChild { get; set; }

        // Thêm thuộc tính này
        public string BuildingName { get; set; }



        // Danh sách dùng cho dropdown
        public IEnumerable<Building> Buildings { get; set; }
        public IEnumerable<Floor> Floors { get; set; }
        public IEnumerable<RoomType> RoomTypes { get; set; }
    }
}
