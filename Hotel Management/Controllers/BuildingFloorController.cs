﻿using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class BuildingFloorController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();

        // GET: BuildingFloor
        public ActionResult Index(string searchString)
        {
            var buildings = db.Buildings.Include(b => b.Floors).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                int searchId;
                if (int.TryParse(searchString, out searchId))
                {
                    buildings = buildings.Where(b => b.BuildingID == searchId ||
                                             b.BuildingName.Contains(searchString) ||
                                             b.Address.Contains(searchString));
                }
                else
                {
                    buildings = buildings.Where(b => b.BuildingName.Contains(searchString) ||
                                             b.Address.Contains(searchString));
                }
            }

            var model = buildings.ToList().Select(b => new BuildingFloorViewModel
            {
                BuildingID = b.BuildingID,
                BuildingName = b.BuildingName,
                Address = b.Address,
                Floors = b.Floors.Select(f => new Floor
                {
                    FloorID = f.FloorID,
                    FloorNumber = f.FloorNumber,
                    TotalRoomsOfFloor = f.TotalRoomsOfFloor
                }).ToList()
            }).ToList();

            return View(model);
        }

        // GET: BuildingFloor/Create
        public ActionResult Create()
        {
            return View(new BuildingFloorViewModel
            {
                Floors = new List<Floor>
        {
            new Floor(),
        }
            });
        }


        // POST: BuildingFloor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuildingFloorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var building = new Building
                {
                    BuildingName = model.BuildingName,
                    Address = model.Address
                    
                };
                db.Buildings.Add(building);
                db.SaveChanges();

                if (model.Floors != null)
                {
                    foreach (var floor in model.Floors)
                    {
                        if (floor != null)
                        {
                            floor.BuildingID = building.BuildingID;
                            db.Floors.Add(floor);
                            
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: BuildingFloor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Building building = db.Buildings.Include(b => b.Floors).FirstOrDefault(b => b.BuildingID == id);
            if (building == null)
            {
                return HttpNotFound();
            }
            var model = new BuildingFloorViewModel
            {
                BuildingID = building.BuildingID,
                BuildingName = building.BuildingName,
                Address = building.Address,
                Floors = building.Floors.Select(f => new Floor
                {
                    FloorID = f.FloorID,
                    FloorNumber = f.FloorNumber,
                    TotalRoomsOfFloor = f.TotalRoomsOfFloor
                }).ToList()
            };
            return View(model);
        }

        // POST: BuildingFloor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BuildingFloorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var building = db.Buildings.Find(model.BuildingID);
                if (building != null)
                {
                    building.BuildingName = model.BuildingName;
                    building.Address = model.Address;

                    var existingFloors = db.Floors.Where(f => f.BuildingID == model.BuildingID).ToList();
                    var modelFloors = model.Floors ?? new List<Floor>();

                    foreach (var existingFloor in existingFloors)
                    {
                        var modelFloor = modelFloors.FirstOrDefault(f => f.FloorID == existingFloor.FloorID);
                        if (modelFloor != null)
                        {
                            existingFloor.FloorNumber = modelFloor.FloorNumber;
                            existingFloor.TotalRoomsOfFloor = modelFloor.TotalRoomsOfFloor;
                            db.Entry(existingFloor).State = EntityState.Modified;
                            modelFloors.Remove(modelFloor);
                        }
                        else
                        {
                            db.Floors.Remove(existingFloor);
                        }
                    }

                    foreach (var newFloor in modelFloors)
                    {
                        if (newFloor != null)
                        {
                            newFloor.BuildingID = model.BuildingID;
                            db.Floors.Add(newFloor);
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: BuildingFloor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Building building = db.Buildings.Find(id);
            if (building == null)
            {
                return HttpNotFound();
            }
            var model = new BuildingFloorViewModel
            {
                BuildingID = building.BuildingID,
                BuildingName = building.BuildingName,
                Address = building.Address,
                Floors = building.Floors.Select(f => new Floor
                {
                    FloorID = f.FloorID,
                    FloorNumber = f.FloorNumber,
                    TotalRoomsOfFloor = f.TotalRoomsOfFloor
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var building = db.Buildings.Include("Floors").FirstOrDefault(b => b.BuildingID == id);
            if (building != null)
            {
                // Xóa các tầng
                foreach (var floor in building.Floors.ToList())
                {
                    db.Floors.Remove(floor);
                }

                // Xóa tòa nhà
                db.Buildings.Remove(building);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Tòa nhà đã được xóa thành công.";
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