using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class StaffAccManagementController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();

        // GET: StaffAccManagement
        [HttpGet]
        public ActionResult Index()
        {
            var staffs = db.Staffs.Include("s => s.Account").Select(s => new StaffViewModel
            {
                AccountID = s.AccountID,
                Username = s.Account.Username,
                Fullname = s.FullName,
                CCCD = s.CCCD,
                Email = s.Email,
                Phone = s.Phone,
                Position = s.Position,
                Status = s.Account.Status
            }).ToList();

            return View(staffs);
        }
        [HttpPost]
        public ActionResult ToggleStatus(int? id)
        {
            var account = db.Accounts.Find(id);

            if (account == null)
            {
                return HttpNotFound();
            }
            if (account.Status != "Blocked")
            {
                account.Status = "Blocked";
            }
            else
            {
                account.Status = "Active";
            }
            //account.Status = !account.Status;
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: StaffAccManagement/Details/{id}
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var staff = db.Staffs
                .Include("s => s.Account")
                .Where(s => s.AccountID == id)
                .Select(s => new StaffViewModel
                {
                    AccountID = s.AccountID,
                    Username = s.Account.Username,
                    Fullname = s.FullName,
                    CCCD = s.CCCD,
                    Email = s.Email,
                    Phone = s.Phone,
                    Position = s.Position,
                    Status = s.Account.Status
                }).FirstOrDefault();

            if (staff == null)
            {
                return HttpNotFound();
            }

            return View(staff);
        }
    }
}