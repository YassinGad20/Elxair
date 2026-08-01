using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElxairContext _context;
        private readonly UserService us;
        private readonly CartService cs;
        private readonly OrderService os;

        public AccountController(
            ElxairContext context,
            UserService userService,
            CartService cartService,
            OrderService orderService)
        {
            _context = context;
            us = userService;
            cs = cartService;
            os = orderService;
        }

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

            HttpContext.Session.Clear();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            HttpContext.Session.Remove("GuestToken");
            HttpContext.Session.Remove("IsGuest");

            TempData["Success"] = $"Welcome back, {user.Name}!";

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // Guest Login
        public IActionResult ContinueAsGuest()
        {
            HttpContext.Session.Clear();

            HttpContext.Session.SetString("IsGuest", "true");
            HttpContext.Session.SetString("GuestToken", Guid.NewGuid().ToString());

            TempData["Success"] = "You are browsing as a guest.";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("IsGuest") == "true")
            {
                int? userId = HttpContext.Session.GetInt32("UserId");

                if (userId.HasValue)
                {
                    // امسح السلة لو موجودة
                    cs.ClearCart(userId.Value);

                    // هل الـ Guest عمل Order قبل كده؟
                    bool hasOrders = _context.Orders.Any(o => o.UserId == userId.Value);

                    // لو معملش Orders احذفه نهائياً
                    if (!hasOrders)
                    {
                        var guest = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

                        if (guest != null)
                        {
                            _context.Users.Remove(guest);
                            _context.SaveChanges();
                        }
                    }
                }
            }

            HttpContext.Session.Clear();

            TempData["Success"] = "Logged out successfully.";

            return RedirectToAction("Login");
        }
    }
}