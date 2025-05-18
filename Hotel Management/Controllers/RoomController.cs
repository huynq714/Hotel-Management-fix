using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hotel_Management.Models; // Đảm bảo rằng bạn đã import namespace của các model
using System.Data.Entity; // Import namespace này nếu bạn dùng Entity Framework

namespace Hotel_Management.Controllers
{
    public class RoomController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities(); // Khởi tạo database context

        
        public ActionResult Index(string sortBy)
        {
            var rooms = db.Rooms
                .Include(r => r.Building)
                .Include(r => r.RoomType)
                .ToList();

            var model = rooms.Select(r => new RoomViewModel
            {
                RoomID = r.RoomID,
                BuildingID = (int)r.BuildingID,
                RoomNumber = r.RoomNumber,
                RoomTypeID = (int)r.RoomTypeID,
                Status = r.Status,
                RoomTypeName = r.RoomType.TypeName,
                RoomPrice = (int)r.RoomType.Price,
                MaxAdult = (int)r.RoomType.MaxAdult,
                MaxChild = (int)r.RoomType.MaxChild,
                BuildingName = r.Building.BuildingName
            });

            switch (sortBy)
            {
                case "buildingName":
                    model = model.OrderBy(r => r.BuildingName);
                    break;
                case "buildingId":
                    model = model.OrderBy(r => r.BuildingID);
                    break;
                case "price":
                    model = model.OrderBy(r => r.RoomPrice);
                    break;
                case "status":
                    model = model.OrderBy(r => r.Status);
                    break;
            }

            return View(model.ToList());
        }



        // GET: Room/Create
        // Hiển thị form tạo phòng mới
        public ActionResult Create(int? buildingId)
        {
            // Khởi tạo view model
            var model = new RoomViewModel
            {
                Buildings = db.Buildings.ToList(), // Lấy danh sách các tòa nhà để hiển thị trong dropdown
                RoomTypes = db.RoomTypes.ToList(), // Lấy danh sách các loại phòng để hiển thị trong dropdown
                Floors = new List<Floor>()         //Khởi tạo để không bị lỗi null
            };


            // Truyền dữ liệu model cho view
            return View(model);
        }

        // POST: Room/Create
        // Xử lý việc tạo phòng mới
        [HttpPost]
        [ValidateAntiForgeryToken] // Ngăn chặn tấn công giả mạo request
        public ActionResult Create(RoomViewModel model)
        {
            // Kiểm tra xem dữ liệu nhập vào có hợp lệ không
            if (ModelState.IsValid)
            {
                // Tạo một đối tượng Room từ dữ liệu trong view model
                var room = new Room
                {
                    BuildingID = model.BuildingID,
                    RoomNumber = model.RoomNumber,
                    RoomTypeID = model.RoomTypeID,
                    Status = model.Status
                };


                // Thêm đối tượng Room vào database
                db.Rooms.Add(room);
                db.SaveChanges(); // Lưu thay đổi vào database

                // Chuyển hướng đến trang danh sách phòng
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, trả lại view Create với dữ liệu đã nhập để người dùng sửa
            //   Chú ý:  Phải lấy lại danh sách Buildings, Floors, RoomTypes để hiển thị lại các dropdown
            model.Buildings = db.Buildings.ToList();
            model.Floors = db.Floors.ToList();  // giữ lại để hiển thị tầng
            model.RoomTypes = db.RoomTypes.ToList();
            return View(model);
        }

        // GET: Room/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            var model = new RoomViewModel
            {
                RoomID = room.RoomID,
                BuildingID = (int)room.BuildingID,
                RoomNumber = room.RoomNumber,
                RoomTypeID = (int)room.RoomTypeID,
                Status = room.Status,
                Buildings = db.Buildings.ToList(),
                RoomTypes = db.RoomTypes.ToList()
            };

            // Tạo danh sách tầng từ 1 đến tầng lớn nhất của Building hiện tại
            var buildingId = room.BuildingID;
            if (buildingId.HasValue)
            {
                var selectedBuilding = db.Buildings.FirstOrDefault(b => b.BuildingID == buildingId);
                if (selectedBuilding != null)
                {
                    // Lấy tầng lớn nhất của tòa nhà được chọn
                    int maxFloorNumber = (int)db.Floors
                        .Where(f => f.BuildingID == buildingId)
                        .Max(f => f.FloorNumber);

                    // Tạo danh sách tầng từ 1 đến tầng lớn nhất
                    var floors = new List<Floor>();
                    for (int i = 1; i <= maxFloorNumber; i++)
                    {
                        floors.Add(new Floor { FloorNumber = i });
                    }
                    model.Floors = floors;
                }
            }

            return View(model);
        }


        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                Room room = db.Rooms.Find(model.RoomID);
                if (room == null)
                {
                    return HttpNotFound();
                }

                room.BuildingID = model.BuildingID;
                room.RoomNumber = model.RoomNumber;
                room.RoomTypeID = model.RoomTypeID;
                room.Status = model.Status;

                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại dữ liệu dropdown
            model.Buildings = db.Buildings.ToList();
            model.RoomTypes = db.RoomTypes.ToList();
            model.Floors = db.Floors.Where(f => f.BuildingID == model.BuildingID).ToList();

            return View(model);
        }


        // GET: Room/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // Lấy phòng cần xóa cùng thông tin liên quan
            Room room = db.Rooms
                .Include(r => r.Building)
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomID == id);

            if (room == null)
            {
                return HttpNotFound();
            }

            // Tạo ViewModel
            var model = new RoomViewModel
            {
                RoomID = room.RoomID,
                BuildingID = (int)room.BuildingID,
                RoomNumber = room.RoomNumber,
                RoomTypeID = (int)room.RoomTypeID,
                Status = room.Status,
                RoomTypeName = room.RoomType.TypeName,
                BuildingName = room.Building.BuildingName,
                Buildings = db.Buildings.ToList(),
                RoomTypes = db.RoomTypes.ToList(),
                Floors = new List<Floor>() // sẽ gán sau
            };

            return View(model);
        }
        

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            if (room != null)
            {
                db.Rooms.Remove(room);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        


    }
}

