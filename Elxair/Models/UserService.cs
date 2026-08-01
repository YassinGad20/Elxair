using Microsoft.AspNetCore.Http;

namespace Elxair.Models
{
    public class UserService
    {
        private readonly ElxairContext db;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(
            ElxairContext db,
            IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Register(User user)
        {
            user.Role = "User";
            user.Cart = new Cart();
            db.Users.Add(user);
            db.SaveChanges();
        }

        public User Login(string email, string password)
        {
            return db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool EmailExists(string email)
        {
            return db.Users.Any(u => u.Email == email);
        }
        public int GetCurrentUserId()
        {
            var session = httpContextAccessor.HttpContext!.Session;

            int? userId = session.GetInt32("UserId");

            if (userId.HasValue)
                return userId.Value;

            if (session.GetString("IsGuest") != "true")
                throw new Exception("No logged in user found.");

            if (string.IsNullOrEmpty(session.GetString("GuestToken")))
            {
                session.SetString("GuestToken", Guid.NewGuid().ToString());
            }

            var guest = new User
            {
                Name = $"Guest_{Guid.NewGuid():N}".Substring(0, 14),
                Email = $"guest_{Guid.NewGuid():N}@elxair.local",
                Password = "",
                Role = "Guest",
                Cart = new Cart()
            };

            db.Users.Add(guest);
            db.SaveChanges();

            session.SetInt32("UserId", guest.Id);
            session.SetString("UserName", guest.Name);
            session.SetString("UserRole", guest.Role);
            session.SetString("IsGuest", "true");

            return guest.Id;
        }
    }
}