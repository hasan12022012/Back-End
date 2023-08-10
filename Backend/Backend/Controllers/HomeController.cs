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
                .Include(m => m.ProductGenres)
                .Take(6)
                .ToListAsync();

            IEnumerable<Genre> genres = await _context.Genres
                .Where(m => !m.IsDeleted)
            .ToListAsync();

            Banner banner = await _context.Banners
                .Where(m => !m.IsDeleted)
                .FirstOrDefaultAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                Products = products,
                Genres = genres,
                Banner = banner
            };

            ViewBag.Ratings = await _context.Ratings.Include(r => r.Comment).ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ShowMore(int skip)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductImages)
                .Include(m => m.ProductGenres)
                .Include(m => m.Author)
                .Skip(skip)
                .Take(6)
                .ToListAsync();

            ViewBag.Ratings = await _context.Ratings.Include(r => r.Comment).ToListAsync();

            HomeVM model = new()
            {
                Products = products
            };

            return PartialView("_HomeProductPartial", model);
        }
    }
}