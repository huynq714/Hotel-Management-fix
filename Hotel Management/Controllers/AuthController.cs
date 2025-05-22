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
                var existing = db.Accounts.FirstOrDefault(a => a.Username == model.UserName);
                if (existing != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                var newAccount = new Account
                {
                    Username = model.UserName,
                    PasswordHash = HashPassword(model.Password),
                    Role = "Customer",
                    Status = "Active"
                };
                db.Accounts.Add(newAccount);
                db.SaveChanges();

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

        // GET: Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.FirstOrDefault(a => a.Username == model.UserName);

                if (account == null)
                {
                    ModelState.AddModelError("UserName", "Tài khoản không tồn tại.");
                    return View(model);
                }

                if (account.Status != "Active")
                {
                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa.");
                    return View(model);
                }

                string hashedPassword = HashPassword(model.Password);
                if (account.PasswordHash != hashedPassword)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng, vui lòng nhập lại.");
                    return View(model);
                }

                // Đăng nhập thành công
                Session["User"] = account.Username;
                Session["Role"] = account.Role;
                Session["AccountID"] = account.AccountID;

                if (account.Role == "Admin")
                {
                    var admin = db.Admins.FirstOrDefault(a => a.AccountID == account.AccountID);
                    if (admin != null) Session["FullName"] = admin.FullName;
                }
                else if (account.Role == "Staff")
                {
                    var staff = db.Staffs.FirstOrDefault(a => a.AccountID == account.AccountID);
                    if (staff != null) Session["FullName"] = staff.FullName;
                }
                else
                {
                    var customer = db.Customers.FirstOrDefault(c => c.AccountID == account.AccountID);
                    if (customer != null) Session["FullName"] = customer.FullName;
                }

                // 1. Nếu có returnUrl → quay về đó (ví dụ: click "Đặt phòng ngay")
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // 2. Nếu là Admin hoặc Staff → về Home/Index
                if (account.Role == "Admin" || account.Role == "Staff")
                {
                    return RedirectToAction("Index", "Home");
                }

                // 3. Nếu là Customer đăng nhập bình thường → về Home/Index
                return RedirectToAction("Index", "Home");
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

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}
