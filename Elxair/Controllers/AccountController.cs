using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class AccountController : Controller
    {
        UserService us = new UserService();

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Role = "User";
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
                return View(user);

            if (us.EmailExists(user.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(user);
            }

            us.Register(user);

            TempData["Success"] = "Account created successfully. Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
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

            TempData["Success"] = "Welcome back!";
            return RedirectToAction("Index", "Product");
        }
    }
}