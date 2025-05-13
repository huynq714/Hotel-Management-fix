using Hotel_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel_Management.Controllers
{
    public class AccountController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();

        [HttpGet]
        public ActionResult AccountInfo()
        {
            if (Session["AccountID"] == null) return RedirectToAction("Login", "Auth");
            int id = (int)Session["AccountID"];
            var account = db.Accounts.FirstOrDefault(a => a.AccountID == id);
            if (account == null)
            {
                return HttpNotFound();
            }
            else
            {
                AccountInfoViewModel model = new AccountInfoViewModel();
                if (account.Role == "Admin")
                {
                    var accountInfo = db.Admins.FirstOrDefault(a => a.AccountID == id);
                    model.UserName = account.Username;
                    model.FullName = accountInfo.FullName;
                    model.Phone = accountInfo.Phone;
                }
                else if (account.Role == "Staff")
                {
                    var accountInfo = db.Staffs.FirstOrDefault(a => a.AccountID == id);
                    model.UserName = account.Username;
                    model.FullName = accountInfo.FullName;
                    model.CCCD = accountInfo.CCCD;
                    model.Email = accountInfo.Email;
                    model.Phone = accountInfo.Phone;

                }
                else
                {
                    var accountInfo = db.Customers.FirstOrDefault(a => a.AccountID == id);
                    model.UserName = account.Username;
                    model.FullName = accountInfo.FullName;
                    model.CCCD = accountInfo.CCCD;
                    model.Email = accountInfo.Email;
                    model.Phone = accountInfo.Phone;
                }

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountInfo(AccountInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.FirstOrDefault(a => a.Username == model.UserName);
                if (account == null)
                {
                    return HttpNotFound();
                }

                if (account.Role == "Admin")
                {
                    var existingAdmin = db.Admins.FirstOrDefault(a => a.AccountID == account.AccountID);
                    if (existingAdmin != null)
                    {
                        existingAdmin.FullName = model.FullName;
                        existingAdmin.Phone = model.Phone;
                        db.SaveChanges();
                        TempData["Message"] = "Cập nhật thông tin thành công!"; // Thêm TempData
                        return View(model);
                    }
                    return HttpNotFound();
                }
                else if (account.Role == "Staff")
                {
                    var existingStaff = db.Staffs.FirstOrDefault(a => a.AccountID == account.AccountID);
                    if (existingStaff != null)
                    {
                        existingStaff.FullName = model.FullName;
                        existingStaff.Phone = model.Phone;
                        existingStaff.CCCD = model.CCCD;
                        existingStaff.Email = model.Email;
                        db.SaveChanges();
                        TempData["Message"] = "Cập nhật thông tin thành công!"; // Thêm TempData
                        return View(model);
                    }
                    return HttpNotFound();
                }
                else if (account.Role == "Customer")
                {
                    var existingCustomer = db.Customers.FirstOrDefault(a => a.AccountID == account.AccountID);
                    if (existingCustomer != null)
                    {
                        existingCustomer.FullName = model.FullName;
                        existingCustomer.Phone = model.Phone;
                        existingCustomer.CCCD = model.CCCD;
                        existingCustomer.Email = model.Email;
                        db.SaveChanges();
                        TempData["Message"] = "Cập nhật thông tin thành công!"; // Thêm TempData
                        return View(model);
                    }
                    return HttpNotFound();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            AccountChangePassViewModel model = new AccountChangePassViewModel();
            if (Session["User"] == null) return RedirectToAction("Login", "Auth");
            model.UserName = Session["User"].ToString();
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(AccountChangePassViewModel model)
        {
            model.UserName = Session["User"].ToString();
            if (ModelState.IsValid)
            {
                var account = db.Accounts.FirstOrDefault(a => a.Username == model.UserName);
                if (account == null)
                {
                    return View(model);
                }
                string hashedPassword = HashPassword(model.Password);
                if (account.PasswordHash != hashedPassword)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng, vui lòng nhập lại.");
                    return View(model);
                }
                else
                {
                    account.PasswordHash = HashPassword(model.NewPassword);
                    db.SaveChanges();
                    TempData["Message"] = "Đổi mật khẩu thành công!"; // Thêm TempData
                    return RedirectToAction("ChangePassword", "Account");
                }
            }
            return View(model);
        }
        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
