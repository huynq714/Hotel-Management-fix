using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class RoomSearchController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();

        // Action method cho trang tìm kiếm
        public ActionResult Index()
        {
            // Truyền dữ liệu cần thiết cho dropdown (loại phòng, tòa nhà)
            ViewBag.RoomTypes = db.RoomTypes.ToList();
            ViewBag.Buildings = db.Buildings.ToList();

            var allRooms = db.Rooms
                .Select(r => new RoomSearchResult
                {
                    RoomID = r.RoomID,
                    RoomNumber = r.RoomNumber,
                    TypeName = r.RoomType.TypeName,
                    BuildingName = r.Building.BuildingName,
                    //FloorNumber = r.Floor.FloorNumber,
                    MaxAdult = r.RoomType.MaxAdult,
                    MaxChild = r.RoomType.MaxChild,
                    Price = r.RoomType.Price,
                    Status = r.Status,
                    Address = r.Building.Address
                })
                .ToList();
            return View(allRooms);
        }

        // Action method để xử lý kết quả tìm kiếm
        [HttpPost]
        public ActionResult Index(DateTime? checkInDate, DateTime? checkOutDate, int? numberAdult, int? numberChild, int? roomTypeID, int? buildingID, decimal? price)
        {
            // 1. Validate dữ liệu đầu vào (rất quan trọng)
            if (numberAdult == null || numberChild == null)
            {
                //ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin ngày nhận phòng, ngày trả phòng và số lượng người.");
                //ViewBag.RoomTypes = db.RoomTypes.ToList();
                //ViewBag.Buildings = db.Buildings.ToList();
                //return View(); // Trả về trang tìm kiếm với thông báo lỗi
                numberAdult = 1;
                numberChild = 0;
            }

            if (checkInDate == null || checkOutDate == null)
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin ngày nhận phòng, ngày trả phòng và số lượng người.");
                ViewBag.RoomTypes = db.RoomTypes.ToList();
                ViewBag.Buildings = db.Buildings.ToList();
                return View(); // Trả về trang tìm kiếm với thông báo lỗi
            }

            if (checkInDate > checkOutDate)
            {
                ModelState.AddModelError("", "Ngày trả phòng phải sau ngày nhận phòng.");
                ViewBag.RoomTypes = db.RoomTypes.ToList();
                ViewBag.Buildings = db.Buildings.ToList();
                return View();
            }

            // 2. Xây dựng truy vấn LINQ để tìm kiếm phòng
            var rooms = db.Rooms
                .Where(r =>
                    // Lọc theo loại phòng (nếu được chọn)
                    (!roomTypeID.HasValue || r.RoomTypeID == roomTypeID) &&
                    // Lọc theo tòa nhà (nếu được chọn)
                    (!buildingID.HasValue || r.BuildingID == buildingID) &&
                    // Lọc theo sức chứa
                    r.RoomType.MaxAdult >= numberAdult && r.RoomType.MaxChild >= numberChild
                    // Lọc theo ngày
                    && !db.Bookings.Any(b =>b.RoomID == r.RoomID &&
                        ((checkInDate < b.CheckOutDate && checkOutDate > b.CheckInDate) || 
                         (checkInDate == b.CheckInDate && checkOutDate == b.CheckOutDate))
                         )
                        // TH1: khoảng thời gian đặt phòng mới giao với khoảng thời gian đã đặt
                        // TH2: khoảng thời gian đặt phòng mới trùng với khoảng thời gian đã đặt
                )
                .Select(r => new RoomSearchResult
                {
                    RoomID = r.RoomID,
                    RoomNumber = r.RoomNumber,
                    TypeName = r.RoomType.TypeName,
                    BuildingName = r.Building.BuildingName,
                    //FloorNumber = r.Floor.FloorNumber,
                    MaxAdult = r.RoomType.MaxAdult,
                    MaxChild = r.RoomType.MaxChild,
                    Price = r.RoomType.Price,
                    Status = r.Status,
                    Address = r.Building.Address //Lấy địa chỉ tòa nhà
                });

            // Lọc theo giá (nếu được chọn)
            if (price.HasValue)
            {
                rooms = rooms.Where(r => r.Price <= price);
            }

            // Thực hiện truy vấn và chuyển đổi kết quả sang List
            var results = rooms.ToList();

            // Truyền dữ liệu vào View
            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;
            ViewBag.NumberAdult = numberAdult;
            ViewBag.NumberChild = numberChild;
            ViewBag.RoomTypes = db.RoomTypes.ToList();
            ViewBag.Buildings = db.Buildings.ToList();

            return View(results); // Truyền danh sách kết quả tìm kiếm đến View
        }

        // Action method cho trang chi tiết phòng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var room = db.Rooms
                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy phòng
            }
            var roomDetail = new RoomSearchResult
            {
                RoomID = room.RoomID,
                RoomNumber = room.RoomNumber,
                TypeName = room.RoomType.TypeName,
                BuildingName = room.Building.BuildingName,
                //FloorNumber = room.Floor.FloorNumber,
                MaxAdult = room.RoomType.MaxAdult,
                MaxChild = room.RoomType.MaxChild,
                Price = room.RoomType.Price,
                Status = room.Status,
                Address = room.Building.Address
            };
            if (TempData["BookingSuccess"] != null) {
                ViewBag.BookingSuccess = TempData["BookingSuccess"];
            }
            if (TempData["BookingError"] != null)
            {

                ViewBag.BookingError = TempData["BookingError"].ToString();
            }
            return View(roomDetail);
        }
    }
}