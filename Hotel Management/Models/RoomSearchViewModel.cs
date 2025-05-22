using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Management.Models
{
    public class RoomSearchResult
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public string TypeName { get; set; }
        public string BuildingName { get; set; }
        public int? MaxAdult { get; set; }
        public int? MaxChild { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public string Address { get; set; } // Địa chỉ của tòa nhà
    }
}