using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            IEnumerable<Product> products = await _context.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .ToListAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                Products = products

            };

            return View(model);
        }
    }
}