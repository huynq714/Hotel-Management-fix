
using System;
using System.Linq;
using System.Web.Mvc;
using Hotel_Management.Models;


namespace Hotel_Management.Controllers
{   

    public class AuthController : Controller
    {
        private Hotel_ManagementEntities db = new Hotel_ManagementEntities();
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Username
                var existing = db.Accounts.FirstOrDefault(a => a.Username == model.UserName);
                if (existing != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Tạo Account mới
                var newAccount = new Account
                {
                    Username = model.UserName,
                    PasswordHash = HashPassword(model.Password), // Gợi ý mã hóa mật khẩu
                    Role = "Customer",
                    Status = "Active"
                };
                db.Accounts.Add(newAccount);
                db.SaveChanges();

                // Tạo Customer gắn với Account
                var customer = new Customer
                {
                    FullName = model.FullName,
                    CCCD = model.CCCD,
                    Email = model.Email,
                    Phone = model.Phone,
                    AccountID = newAccount.AccountID
                };
                db.Customers.Add(customer);
                db.SaveChanges();

                TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }



        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.Password);
                var account = db.Accounts.FirstOrDefault(a =>
                    a.Username == model.UserName &&
                    a.PasswordHash == hashedPassword &&
                    a.Status == "Active");

                if (account != null)
                {
                    Session["User"] = account.Username;
                    Session["Role"] = account.Role;
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
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
