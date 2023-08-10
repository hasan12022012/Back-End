using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.AuthorViewModels;
using Backend.ViewModels.BlogCategoryViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogCategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogCategoryController(AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<BlogCategory> categories = await _context.BlogCategories
                  .Where(m => !m.IsDeleted)
                  .Include(m => m.Blogs)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<BlogCategoryListVM> mapDatas = GetMapDatas(categories);
            int count = await GetPageCount(take);

            Paginate<BlogCategoryListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategoryCreateVM category)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            BlogCategory newCategory = new()
            {
                Name = category.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.BlogCategories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            BlogCategory dbCategory = await GetByIdAsync((int)id);

            ViewBag.Blogs = await GetProductsAsync((int)id);

            BlogCategoryUpdateVM updatedCategory = new()
            {
                Id = dbCategory.Id,
                Name = dbCategory.Name,
                Blogs = dbCategory.Blogs
            };

            return View(updatedCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, BlogCategoryUpdateVM updatedCategory)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.Blogs = await GetProductsAsync((int)id);

            if (!ModelState.IsValid) return View(updatedCategory);

            BlogCategory dbCategory = await GetByIdAsync((int)id);

            dbCategory.Name = updatedCategory.Name;
            dbCategory.UpdatedAt = DateTime.UtcNow;
            dbCategory.UpdatedBy = user.UserName;

            if (updatedCategory.BlogIds == null)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<Blog> blogs = await _context.Blogs
                  .Where(m => !m.IsDeleted)
                  .ToListAsync();

            foreach (var blogId in updatedCategory.BlogIds)
            {
                foreach (var blog in blogs)
                {
                    if (blog.Id == blogId)
                    {
                        blog.BlogCategoryId = dbCategory.Id;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            bool result = false;

            if (blogId == null) return BadRequest();

            Blog blog = await _context.Blogs
                .Where(m => m.Id == blogId)
                .FirstOrDefaultAsync();

            if (blog == null) return NotFound();

            BlogCategory category = await _context.BlogCategories.Include(m => m.Blogs).FirstOrDefaultAsync(m => m.Id ==blog.BlogCategoryId);

            if (category.Blogs.Count > 1)
            {
                category.Blogs.Remove(blog);
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

            BlogCategory category = await GetByIdAsync((int)id);

            if (category == null) return NotFound();

            BlogCategoryDetailVM categoryDetail = new()
            {
                Name = category.Name,
                Blogs = category.Blogs
            };

            return View(categoryDetail);
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

            BlogCategory category = await _context.BlogCategories
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<int> GetPageCount(int take)
        {
            int categoryCount = await _context.BlogCategories.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)categoryCount / take);
        }

        private List<BlogCategoryListVM> GetMapDatas(List<BlogCategory> categories)
        {
            List<BlogCategoryListVM> categoryList = new();

            foreach (var category in categories)
            {
                BlogCategoryListVM newCategory = new()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Blogs = category.Blogs
                };

                categoryList.Add(newCategory);
            }

            return categoryList;
        }

        private async Task<SelectList> GetProductsAsync(int? id)
        {
            IEnumerable<Blog> blogs = await _context.Blogs.Where(m => !m.IsDeleted && m.BlogCategoryId != id).ToListAsync();

            return new SelectList(blogs, "Id", "Name");
        }

        private async Task<BlogCategory> GetByIdAsync(int id)
        {
            return await _context.BlogCategories
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.Blogs)
                .FirstOrDefaultAsync();
        }
    }
}
