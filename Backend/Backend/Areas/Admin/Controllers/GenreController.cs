using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.GenreViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public GenreController(AppDbContext context,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Genre> genres = await _context.Genres
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.ProductGenres)
                  .ThenInclude(m => m.Product)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<GenreListVM> mapDatas = GetMapDatas(genres);
            int count = await GetPageCount(take);

            Paginate<GenreListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await GetProductsAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreCreateVM genre)
        {
            ViewBag.Products = await GetProductsAsync();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Genre newGenre = new()
            {
                Name = genre.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Genres.AddAsync(newGenre);
            await _context.SaveChangesAsync();

            foreach (var productId in genre.ProductIds)
            {
                ProductGenre product = new()
                {
                    GenreId = newGenre.Id,
                    ProductId = productId
                };

                await _context.ProductGenres.AddAsync(product);
            }

            _context.Genres.Update(newGenre);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Genre dbGenre = await GetByIdAsync((int)id);

            ViewBag.Products = await GetProductsAsync();

            GenreUpdateVM updatedGenre = new()
            {
                Name = dbGenre.Name,
                Products = dbGenre.ProductGenres
            };

            return View(updatedGenre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, GenreUpdateVM updatedGenre)
        {
            ViewBag.Products = await GetProductsAsync();

            if (!ModelState.IsValid) return View(updatedGenre);

            Genre dbGenre = await GetByIdAsync(id);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            dbGenre.Name = updatedGenre.Name;
            dbGenre.UpdatedAt = DateTime.UtcNow;
            dbGenre.UpdatedBy = user.UserName;

            List<ProductGenre> product = await _context.ProductGenres
                   .Where(m => m.GenreId == id)
                   .ToListAsync();

            if (updatedGenre.ProductIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var item in product)
            {
                _context.ProductGenres.Remove(item);
            }

            foreach (var productId in updatedGenre.ProductIds)
            {
                ProductGenre product1 = new()
                {
                    ProductId = productId
                };

                dbGenre.ProductGenres.Add(product1);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Genre genre = await GetByIdAsync((int)id);

            if (genre == null) return NotFound();

            GenreDetailVM genreDetail = new()
            {
                Name = genre.Name,
                Products = genre.ProductGenres
            };

            return View(genreDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            Genre genre = await _context.Genres
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductGenres)
                .FirstOrDefaultAsync();

            if (genre == null) return NotFound();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            genre.IsDeleted = true;
            genre.DeletedAt = DateTime.UtcNow;
            genre.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int genreCount = await _context.Genres.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)genreCount / take);
        }

        private List<GenreListVM> GetMapDatas(List<Genre> genres)
        {
            List<GenreListVM> genreList = new();

            foreach (var genre in genres)
            {
                GenreListVM newGenre = new()
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Books = genre.ProductGenres,
                };

                genreList.Add(newGenre);
            }

            return genreList;
        }

        private async Task<SelectList> GetProductsAsync()
        {
            IEnumerable<Product> products = await _context.Products.Where(m => !m.IsDeleted).ToListAsync();

            return new SelectList(products, "Id", "Name");
        }

        private async Task<Genre> GetByIdAsync(int id)
        {
            return await _context.Genres
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductGenres)
                .ThenInclude(m => m.Product)
                .FirstOrDefaultAsync();
        }
    }
}
