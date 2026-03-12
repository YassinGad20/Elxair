using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class AccountController : Controller
    {
        UserService us = new UserService();

        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // التأكد إن الإيميل مش موجود
            if (us.EmailExists(user.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(user);
            }

            // تحديد نوع المستخدم
            user.Role = "User";

            // تسجيل المستخدم
            us.Register(user);

            return RedirectToAction("Login");
        }
        public IActionResult Login(string email, string password)
        {
            var user = us.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Product");
        }

    }
}
