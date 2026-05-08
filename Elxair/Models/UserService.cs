using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Elxair.Models
{
    public class UserService
    {
        private readonly ElxairContext db;

        public UserService(ElxairContext db)
        {
            this.db = db;
        }
        public void Register(User user)
        {
            user.Role = "User";
            // أول ما يسجل، بنكريت له كارت فاضية مربوطة بيه
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

    }
}
