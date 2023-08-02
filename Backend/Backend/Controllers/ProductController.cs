using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .ToListAsync();

            IEnumerable<Genre> genres = await _appDbContext.Genres
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            IEnumerable<Author> authors = await _appDbContext.Authors
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            ProductVM model = new()
            {
                Products = products,
                Genres = genres,
                Authors = authors
            };

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _appDbContext.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .FirstOrDefaultAsync();

            ProductVM model = new()
            {
                Product = product
            };

            return View(model);
        }

        public async Task<IActionResult> Search(string search)
        {
            ProductVM searchProduct = new();

            if (search != null)
            {
                searchProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted && m.Name.ToLower()
                    .Contains(search.ToLower()) && !m.IsDeleted)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }
            else
            {
                searchProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            return PartialView("_ProductsPartial", searchProduct);
        }

        public async Task<IActionResult> FilterPrice(int? min, int? max)
        {
            ProductVM filterProduct = new();

            if (min != null && max != null)
            {
                filterProduct.Products = await _appDbContext.Products
                    .Where(p => !p.IsDeleted && p.Price >= min && p.Price <= max)
                    .Include(p => p.ProductImages)
                    .Include(p => p.Author)
                    .ToListAsync();
            }
            else
            {
                filterProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            return PartialView("_ProductsPartial", filterProduct);
        }
    }
}
