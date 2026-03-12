namespace Elxair.Models
{
    public class ProductService
    {
        ElxairContext db = new ElxairContext();


        public List<Perfume> GetAllPerfumes()
        {
            return db.Perfumes.ToList();
        }

        public Perfume GetById(int id)
        {
            return db.Perfumes.Find(id);
        }

        public void Add(Perfume perfume)
        {
            db.Perfumes.Add(perfume);
            db.SaveChanges();
        }

        public void Update(Perfume perfume)
        {
            db.Perfumes.Update(perfume);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var p = db.Perfumes.Find(id);
            db.Perfumes.Remove(p);
            db.SaveChanges();
        }


    }
}
