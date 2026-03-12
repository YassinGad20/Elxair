using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class AdminController : Controller
    {

        ProductService ps = new ProductService();
        OrderService os = new OrderService();


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Products()
        {
            var perfumes = ps.GetAllPerfumes();
            return View(perfumes);
        }

        public IActionResult Create()
        {
            return View();
        }

        
        public IActionResult Create(Perfume perfume)
        {
            ps.Add(perfume);
            return RedirectToAction("Products");
        }

        public IActionResult Edit(int id)
        {
            var perfume = ps.GetById(id);
            return View(perfume);
        }

        
        public IActionResult Edit(Perfume perfume)
        {
            ps.Update(perfume);
            return RedirectToAction("Products");
        }

        public IActionResult Delete(int id)
        {
            ps.Delete(id);
            return RedirectToAction("Products");
        }

        public IActionResult GetAllOrders()
        {
            var orders = os.GetAllOrders();
            return View(orders);
        }

    }
}
