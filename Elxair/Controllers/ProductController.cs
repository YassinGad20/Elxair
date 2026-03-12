using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            ProductService ps = new ProductService();
            List<Perfume> perfumes = ps.GetAllPerfumes();
            return View( perfumes);
        }

        public IActionResult PerfumeDetails (int id)
        {
            ProductService ps = new ProductService();
            Perfume perfume = ps.GetById(id);
            return View(perfume);
        }
    }
}
