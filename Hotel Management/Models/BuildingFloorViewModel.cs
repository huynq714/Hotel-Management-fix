using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel_Management.Models
{
   
    public class BuildingFloorViewModel
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string Address { get; set; }

        public int FloorID { get; set; }  // Include FloorID
       
        public string FloorName { get; set; } 
        public int TotalRoomsOfFloor { get; set; } 
        public List<Floor> Floors { get; set; } = new List<Floor>(); // Initialize the list

        
    }



}