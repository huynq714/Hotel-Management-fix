using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class BuildingFloorController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();

        // Trang danh sách Tòa nhà và các tầng của nó
        public ActionResult Index()
        {
            // Eager load các tầng (floors) của các tòa nhà
            var buildings = db.Buildings.Include("Floors").ToList();

            var model = buildings.Select(b => new BuildingFloorViewModel
            {
                BuildingID = b.BuildingID,
                BuildingName = b.BuildingName,
                Address = b.Address,
                Floors = b.Floors.ToList() // Dự liệu các tầng tương ứng với mỗi tòa nhà
            }).ToList();

            return View(model);
        }

        // Thêm mới Tòa nhà và các Tầng
        [HttpGet]
        public ActionResult Create()
        {
            var model = new BuildingFloorViewModel { Floors = new List<Floor>() };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BuildingFloorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Thêm tòa nhà
                var building = new Building
                {
                    BuildingName = model.BuildingName,
                    Address = model.Address
                };
                db.Buildings.Add(building);
                db.SaveChanges(); // Lưu vào database để lấy BuildingID

                // Thêm các tầng liên quan
                if (model.Floors != null)
                {
                    foreach (var floor in model.Floors)
                    {
                        if (floor != null)
                        {
                            floor.BuildingID = building.BuildingID; // Gán BuildingID cho tầng
                            db.Floors.Add(floor);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(model); // Nếu model không hợp lệ, trả về lại trang tạo
        }

        // Xóa Tòa nhà và tất cả các tầng liên quan
        public ActionResult Delete(int id)
        {
            var building = db.Buildings.Find(id);
            if (building != null)
            {
                var floors = db.Floors.Where(f => f.BuildingID == id).ToList();
                db.Floors.RemoveRange(floors);
                db.Buildings.Remove(building);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Chỉnh sửa Tòa nhà và Tầng
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var building = db.Buildings.Include("Floors").FirstOrDefault(b => b.BuildingID == id);
            if (building == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy tòa nhà, trả về lỗi 404
            }

            var model = new BuildingFloorViewModel
            {
                BuildingID = building.BuildingID,
                BuildingName = building.BuildingName,
                Address = building.Address,
                Floors = building.Floors.ToList()
            };

            return View(model); // Trả về trang chỉnh sửa với dữ liệu hiện tại
        }

        [HttpPost]
        public ActionResult Edit(BuildingFloorViewModel model)
        {
            // Kiểm tra dữ liệu trước khi lưu
            if (!ModelState.IsValid)
            {
                return View(model); // Nếu không hợp lệ, trả về view và hiển thị lỗi
            }

            var building = db.Buildings.Find(model.BuildingID);
            if (building != null)
            {
                building.BuildingName = model.BuildingName;
                building.Address = model.Address;

                // Cập nhật các tầng
                var existingFloors = db.Floors.Where(f => f.BuildingID == model.BuildingID).ToList();
                var modelFloors = model.Floors ?? new List<Floor>();

                // Cập nhật tầng đã tồn tại
                foreach (var existingFloor in existingFloors)
                {
                    var modelFloor = modelFloors.FirstOrDefault(f => f.FloorID == existingFloor.FloorID);
                    if (modelFloor != null)
                    {
                        existingFloor.FloorNumber = modelFloor.FloorNumber;
                        db.Entry(existingFloor).State = System.Data.Entity.EntityState.Modified;
                        modelFloors.Remove(modelFloor); // Xóa tầng đã cập nhật khỏi danh sách
                    }
                    else
                    {
                        db.Floors.Remove(existingFloor); // Xóa tầng không còn trong model
                    }
                }

                // Thêm tầng mới
                foreach (var newFloor in modelFloors)
                {
                    if (newFloor != null)
                    {
                        newFloor.BuildingID = model.BuildingID;
                        db.Floors.Add(newFloor);
                    }
                }

                // Kiểm tra trước khi lưu
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu có
                    Console.WriteLine(ex.Message);
                    return View(model); // Nếu có lỗi, quay lại view và hiển thị lỗi
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }


    }
}
