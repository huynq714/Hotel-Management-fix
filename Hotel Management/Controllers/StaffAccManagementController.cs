using Hotel_Management.Models;
using System;
using System.Collections.Generic;
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
    }

}