using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Elxair.Models
{
    public class UserService
    {
        ElxairContext db = new ElxairContext();
        public void Register(User user)
        {
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

    }
}
