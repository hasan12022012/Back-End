using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.AuthorViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AuthorController(AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Author> authors = await _context.Authors
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.Products)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<AuthorListVM> mapDatas = GetMapDatas(authors);
            int count = await GetPageCount(take);

            Paginate<AuthorListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorCreateVM author)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Author newAuthor = new()
            {
                Name = author.Name,
                About = author.About,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Author dbAuthor = await GetByIdAsync((int)id);

            ViewBag.Products = await GetProductsAsync((int)id);

            AuthorUpdateVM updatedAuthor = new()
            {
                Id = dbAuthor.Id,
                Name = dbAuthor.Name,
                About = dbAuthor.About,
                Products = dbAuthor.Products
            };

            return View(updatedAuthor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, AuthorUpdateVM updatedAuthor)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Products = await GetProductsAsync((int)id);

            if (!ModelState.IsValid) return View(updatedAuthor);

            Author dbAuthor = await GetByIdAsync((int)id);

            dbAuthor.Name = updatedAuthor.Name;
            dbAuthor.About = updatedAuthor.About;
            dbAuthor.UpdatedAt = DateTime.UtcNow;
            dbAuthor.UpdatedBy = user.UserName;

            if (updatedAuthor.ProductIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<Product> product = await _context.Products
                  .Where(m => !m.IsDeleted)
                  .ToListAsync();

            foreach (var productId in updatedAuthor.ProductIds)
            {
                foreach (var product1 in product)
                {
                    if (product1.Id == productId)
                    {
                        product1.AuthorId = dbAuthor.Id;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            bool result = false;

            if (productId == null) return BadRequest();

            Product product = await _context.Products
                .Where(m => m.Id == productId)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            Author author = await _context.Authors.Include(m => m.Products).FirstOrDefaultAsync(m => m.Id == product.AuthorId);

            if (author.Products.Count > 1)
            {
                author.Products.Remove(product);
                result = true;
            }

            await _context.SaveChangesAsync();

            return Ok(result);
        }

        //[Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Author author = await GetByIdAsync((int)id);

            if (author == null) return NotFound();

            AuthorDetailVM authorDetail = new()
            {
                Name = author.Name,
                Products = author.Products
            };

            return View(authorDetail);
        }

        //[Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Author author = await _context.Authors
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();

            if (author == null) return NotFound();

            author.IsDeleted = true;
            author.DeletedAt = DateTime.UtcNow;
            author.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int authorCount = await _context.Authors.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)authorCount / take);
        }

        private List<AuthorListVM> GetMapDatas(List<Author> authors)
        {
            List<AuthorListVM> authorList = new();

            foreach (var author in authors)
            {
                AuthorListVM newAuthor = new()
                {
                    Id = author.Id,
                    Name = author.Name,
                    About = author.About,
                    Products = author.Products
                };

                authorList.Add(newAuthor);
            }

            return authorList;
        }

        private async Task<SelectList> GetProductsAsync(int? id)
        {
            IEnumerable<Product> products = await _context.Products.Where(m => !m.IsDeleted && m.AuthorId != id).ToListAsync();

            return new SelectList(products, "Id", "Name");
        }

        private async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.Products)
                .FirstOrDefaultAsync();
        }
    }
}
