using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
        public class BookingController : Controller
        {
            private Hotel_ManagementEntities db = new Hotel_ManagementEntities();


        public ActionResult Index()
        {
            if(Session["AccountID"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int AccountId = Convert.ToInt32(Session["AccountID"]); // Lấy AccountID từ Session
            int? customerId = db.Customers.FirstOrDefault(a => a.AccountID == AccountId).CustomerID;
            if (customerId == null)
            {
                return HttpNotFound();
            }
            var bookings = db.Bookings
                .Where(b => b.CustomerID == customerId) // Lọc theo CustomerID
                .ToList()
                .Select(booking => new BookingDetailViewModel
                {
                    BookingID = booking.BookingID,
                    CustomerName = booking.Customer.FullName,
                    RoomID = booking.RoomID,
                    RoomNumber = booking.Room.RoomNumber,
                    RoomType = booking.Room.RoomType.TypeName,
                    BuildingName = booking.Room.Building.BuildingName,
                    BuildingAddress = booking.Room.Building.Address,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    NumberAdult = booking.NumberAdult,
                    NumberChild = booking.NumberChild,
                    TotalPrice = booking.TotalPrice,
                    BookingStatus = booking.BookingStatus,
                    CreatedAt = booking.CreatedAt
                });
            return View(bookings);
        }

        // GET: Booking/Details/5
        public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return HttpNotFound();
                }

                var booking = db.Bookings.FirstOrDefault(m => m.BookingID == id);

                if (booking == null)
                {
                    return HttpNotFound();
                }

                var bookingDetail = new BookingDetailViewModel
                {
                    BookingID = booking.BookingID,
                    CustomerName = booking.Customer.FullName, // Or whatever the customer name property is
                    RoomID = booking.RoomID,
                    RoomNumber = booking.Room.RoomNumber,
                    RoomType = booking.Room.RoomType.TypeName,
                    BuildingName = booking.Room.Building.BuildingName,
                    BuildingAddress = booking.Room.Building.Address,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    NumberAdult = booking.NumberAdult,
                    NumberChild = booking.NumberChild,
                    TotalPrice = booking.TotalPrice,
                    BookingStatus = booking.BookingStatus,
                    CreatedAt = booking.CreatedAt
                };

                return View(bookingDetail);
            }

            // GET: Booking/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return HttpNotFound();
                }

                var booking = db.Bookings.FirstOrDefault(b => b.BookingID == id);
                if (booking == null)
                {
                    return HttpNotFound();
                }

                // Truyền thêm thông tin về phòng để hiển thị trong form chỉnh sửa
                ViewBag.RoomNumber = booking.Room.RoomNumber;
                return View(booking);
            }

            // POST: Booking/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(int id, Booking booking)
            {
                if (id != booking.BookingID)
                {
                    return HttpNotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingBooking = db.Bookings.Find(id);
                        if (existingBooking == null)
                        {
                            return HttpNotFound();
                        }
                        existingBooking.CheckInDate = booking.CheckInDate;
                        existingBooking.CheckOutDate = booking.CheckOutDate;
                        existingBooking.NumberAdult = booking.NumberAdult;
                        existingBooking.NumberChild = booking.NumberChild;


                        //db.Update(existingBooking);
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookingExists(booking.BookingID))
                        {
                            return HttpNotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Details), new { id = booking.BookingID });
                }
                return View(booking);
            }

            // GET: Booking/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return HttpNotFound();
                }

                var booking = db.Bookings.FirstOrDefault(m => m.BookingID == id);
                if (booking == null)
                {
                    return HttpNotFound();
                }

                var bookingDetail = new BookingDetailViewModel
                {
                    BookingID = booking.BookingID,
                    CustomerName = booking.Customer.FullName, // Or whatever the customer name property is
                    RoomID = booking.RoomID,
                    RoomNumber = booking.Room.RoomNumber,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    NumberAdult = booking.NumberAdult,
                    NumberChild = booking.NumberChild,
                    TotalPrice = booking.TotalPrice,
                    BookingStatus = booking.BookingStatus,
                    CreatedAt = booking.CreatedAt
                };
                return View(bookingDetail);
            }

        // POST: Booking/Delete/5
        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            // Chỉ cho phép xoá khi trạng thái là Pending
            if (booking.BookingStatus != "Pending")
            {
                TempData["BookingDeleted"] = "Chỉ có thể xóa các đơn đặt phòng đang chờ xử lý.";
                return RedirectToAction("Index");
            }

            db.Bookings.Remove(booking);
            db.SaveChanges();

            TempData["BookingDeleted"] = "Đơn đặt phòng đã được xóa thành công.";
            return RedirectToAction("Index");
        }






        // POST: Booking/Create
        [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult BookRoom(int RoomId, DateTime CheckInDate, DateTime CheckOutDate, int NumberAdult, int NumberChild)
            {
                if (Session["AccountID"] == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                int? AccoountId = Convert.ToInt32(Session["AccountID"]);
                int? customerId = db.Customers.FirstOrDefault(a => a.AccountID == AccoountId).CustomerID;
                if (customerId == null)
                {
                    return HttpNotFound();
                }

                if(CheckOutDate < CheckInDate)
                {
                    TempData["BookingError"] = "Thời gian trả phòng phải sau ngày nhận phòng!!.";
                    return RedirectToAction("Details", "RoomSearch", new { id = RoomId });

                }

            // Get the room
            var room = db.Rooms.FirstOrDefault(r => r.RoomID == RoomId);
                if (room == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra trùng lặp ngày đặt phòng
                if (db.Bookings.Any(b =>
                    b.RoomID == RoomId &&
                    ((CheckInDate < b.CheckOutDate && CheckOutDate > b.CheckInDate) ||
                     (CheckInDate == b.CheckInDate && CheckOutDate == b.CheckOutDate) ||
                      (b.CheckInDate <= CheckInDate && CheckInDate <= b.CheckOutDate && CheckOutDate >= b.CheckOutDate) ||
                      (b.CheckInDate <= CheckOutDate && CheckOutDate <= b.CheckOutDate && CheckInDate <= b.CheckInDate)
                     )))
                {
                    TempData["BookingError"] = "Phòng đã được đặt trong khoảng thời gian này.";
                    // Truyền lại dữ liệu cần thiết để hiển thị lại trang chi tiết phòng
                    return RedirectToAction("Details", "RoomSearch", new { id = RoomId });
                }

                // Kiểm tra số lượng người
                if (NumberAdult > room.RoomType.MaxAdult || NumberChild > room.RoomType.MaxChild)
                {
                    TempData["BookingError"] = "Số lượng người lớn hoặc trẻ em vượt quá sức chứa của phòng.";
                    return RedirectToAction("Details", "RoomSearch", new { id = RoomId });
                }


            // Create booking
            var booking = new Booking
            {
                RoomID = RoomId,
                CustomerID = customerId,
                CheckInDate = CheckInDate,
                CheckOutDate = CheckOutDate,
                NumberAdult = NumberAdult,
                NumberChild = NumberChild,
                // For demo, set a default CustomerID.  In a real app, you'd get this from the logged-in user.
                TotalPrice = room.RoomType.Price * (decimal)(CheckOutDate - CheckInDate).TotalDays, // Tính toán giá
                BookingStatus = "Pending", // Set initial status
                CreatedAt = DateTime.Now,
                Bills = null
            };
                db.Bookings.Add(booking);
                db.SaveChanges();
                TempData["BookingSuccess"] = "Đặt phòng thành công!";
                
                return RedirectToAction("Details", "RoomSearch", new { id = RoomId });
            }

            // GET: Booking/Bills
            public ActionResult Bills()
            {
                var bills = db.Bookings.Where(b => b.BookingStatus == "Paid") // Lọc theo trạng thái "Paid"
                   .ToList()
                   .Select(booking => new BookingDetailViewModel
                   {
                       BookingID = booking.BookingID,
                       CustomerName = booking.Customer.FullName,
                       RoomID = booking.RoomID,
                       RoomNumber = booking.Room.RoomNumber,
                       RoomType = booking.Room.RoomType.TypeName,
                       BuildingName = booking.Room.Building.BuildingName,
                       BuildingAddress = booking.Room.Building.Address,
                       CheckInDate = booking.CheckInDate,
                       CheckOutDate = booking.CheckOutDate,
                       NumberAdult = booking.NumberAdult,
                       NumberChild = booking.NumberChild,
                       TotalPrice = booking.TotalPrice,
                       BookingStatus = booking.BookingStatus,
                       CreatedAt = booking.CreatedAt
                   });
                return View(bills);
            }

            private bool BookingExists(int id)
            {
                return db.Bookings.Any(e => e.BookingID == id);
            }
        }
    }