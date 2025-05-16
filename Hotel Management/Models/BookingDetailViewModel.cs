using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Management.Models
{
    public class BookingDetailViewModel
    {
        public int? BookingID { get; set; }
        public string CustomerName { get; set; }
        public int? RoomID { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public int? FloorNumber { get; set; }
        public string BuildingName { get; set; }
        public string BuildingAddress { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? NumberAdult { get; set; }
        public int? NumberChild { get; set; }
        public decimal? TotalPrice { get; set; }
        public string BookingStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}