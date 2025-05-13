using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            var staffs = db.Staffs.Include(s => s.Account).Select(s => new StaffViewModel
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
                return RedirectToAction(nameof(Index));
            }

            var staff = db.Staffs
                .Include(s => s.Account)
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

        // POST: StaffAccManagement/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StaffViewModel model)
        {
            if (id != model.AccountID)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                var accountToUpdate = db.Accounts.Find(model.AccountID);
                var staffToUpdate = db.Staffs.FirstOrDefault(s => s.AccountID == model.AccountID);

                if (accountToUpdate == null || staffToUpdate == null)
                {
                    return HttpNotFound();
                }

                accountToUpdate.Status = model.Status;

                staffToUpdate.FullName = model.Fullname;
                staffToUpdate.CCCD = model.CCCD;
                staffToUpdate.Email = model.Email;
                staffToUpdate.Phone = model.Phone;
                staffToUpdate.Position = model.Position;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(model.AccountID))
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = model.AccountID });
            }
            return View(model);
        }

        private bool StaffExists(int? id)
        {
            return db.Staffs.Any(e => e.AccountID == id);
        }

        // POST: StaffAccManagement/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var staffToDelete = await db.Staffs.Include(s => s.Account).FirstOrDefaultAsync(s => s.AccountID == id);
            if (staffToDelete == null)
            {
                return HttpNotFound();
            }

            db.Accounts.Remove(staffToDelete.Account); // Xóa cả account liên quan
            db.Staffs.Remove(staffToDelete);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}