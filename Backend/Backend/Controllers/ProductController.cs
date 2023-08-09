using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .ToListAsync();

            IEnumerable<Genre> genres = await _appDbContext.Genres
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            IEnumerable<Author> authors = await _appDbContext.Authors
                .Where(m => !m.IsDeleted)
                .Include(m => m.Products)
                .ToListAsync();

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

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
            ViewBag.UserId = null;

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.UserId = user.Id;
            }

            Product product = await _appDbContext.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductGenres)
                .ThenInclude(m => m.Genre)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .Include(p => p.Comments.Where(m => m.ProductId == id)).ThenInclude(c => c.AppUser)
                .Include(p => p.Comments.Where(m => m.ProductId == id)).ThenInclude(c => c.Rating)
                .FirstOrDefaultAsync();

            ViewBag.Ratings = _appDbContext.Ratings
                .Where(r => r.Comment.ProductId == id)
                .Include(r => r.Comment)
                .ThenInclude(r => r.Product)
                .ToList();

            ViewBag.CommentCount = await _appDbContext.Comments.Where(m => !m.IsDeleted).CountAsync();

            ViewBag.Product = product;

            return View(product);
        }

        public async Task<IActionResult> Search(string search)
        {
            ProductVM searchProduct = new();

            if (search != null)
            {
                searchProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted && m.Name.ToLower()
                    .Contains(search.ToLower()) && !m.IsDeleted)
                     .Include(m => m.ProductGenres)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }
            else
            {
                searchProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", searchProduct);
        }

        public async Task<IActionResult> FilterGenre(int id)
        {
            ProductVM model = new();

            if (id != null)
            {
                model.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted && m.ProductGenres
                    .Where(m => m.GenreId == id)
                    .FirstOrDefault().GenreId == id)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }
            else
            {
                model.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductImages)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> FilterAuthor(int id)
        {
            ProductVM model = new();

            if (id != null)
            {
                model.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted && m.AuthorId == id)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }
            else
            {
                model.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductImages)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> FilterPrice(int? min, int? max)
        {
            ProductVM filterProduct = new();

            if (min != null && max != null)
            {
                filterProduct.Products = await _appDbContext.Products
                    .Where(p => !p.IsDeleted && p.Price >= min && p.Price <= max)
                    .Include(m => m.ProductGenres)
                    .Include(p => p.ProductImages)
                    .Include(p => p.Author)
                    .ToListAsync();
            }
            else
            {
                filterProduct.Products = await _appDbContext.Products
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.ProductGenres)
                    .Include(m => m.ProductImages)
                    .Include(m => m.Author)
                    .ToListAsync();
            }

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", filterProduct);
        }

        public async Task<IActionResult> SortName()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .OrderBy(m => m.Name)
                .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> SortOld()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .OrderBy(m => m.Id)
                .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> SortNew()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> SortLow()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .OrderBy(m => m.Price)
                .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> SortHigh()
        {
            IEnumerable<Product> products = await _appDbContext.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductGenres)
                .Include(m => m.ProductImages)
                .Include(m => m.Author)
                .OrderByDescending(m => m.Price)
                .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        public async Task<IActionResult> FilterRating(double rating)
        {
            IEnumerable<Product> products = await _appDbContext.Products
            .Where(m => !m.IsDeleted && m.Rating == rating)
            .Include(m => m.ProductGenres)
            .Include(m => m.ProductImages)
            .Include(m => m.Author)
            .ToListAsync();

            ProductVM model = new();
            model.Products = products;

            ViewBag.Ratings = await _appDbContext.Ratings.Include(r => r.Comment).ToListAsync();

            return PartialView("_ProductsPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Comment? com, int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.Identity.IsAuthenticated)
            {
                Product? product = _appDbContext.Products.Include(p => p.Comments).FirstOrDefault(c => c.Id == id);
                if (product == null) return NotFound();
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                Rating ratingRegistered = new();
                Comment commentRegistered = new();

                ratingRegistered.Point = com.Rating.Point;
                commentRegistered.Message = com.Message;
                commentRegistered.AppUser = user;
                commentRegistered.Product = product;
                commentRegistered.Rating = ratingRegistered;
                commentRegistered.CreateDate = DateTime.Now;

                double rateCount = _appDbContext.Ratings.Where(r => r.Comment.ProductId == id).Count();

                if (product.Rating.HasValue)
                {
                    double currentTotalRating = product.Rating.Value * rateCount;
                    double newTotalRating = currentTotalRating + com.Rating.Point;
                    rateCount++;
                    double averageRating = newTotalRating / rateCount;
                    product.Rating = Math.Min(10.0, averageRating);
                }
                else
                {
                    product.Rating = Math.Min(10.0, com.Rating.Point);
                }

                _appDbContext.Comments.Add(commentRegistered);
                _appDbContext.Products.Update(product);
                await _appDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            Comment? comment = await _appDbContext.Comments.FirstOrDefaultAsync(b => b.Id == id);
            _appDbContext.Comments.Remove(comment);

            List<Rating> ratings = _appDbContext.Ratings
                .Where(r => r.Comment.ProductId == id)
                .Include(r => r.Comment)
                .ThenInclude(r => r.Product)
                .ToList();

            await _appDbContext.SaveChangesAsync();

            return PartialView("_RatingPartial", ratings);
        }
    }
}
