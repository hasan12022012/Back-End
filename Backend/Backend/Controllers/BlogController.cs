using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public BlogController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _appDbContext.Blogs
                .Where(m => !m.IsDeleted)
                .Include(m => m.BlogTags)
                .Include(m => m.BlogCategory)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            IEnumerable<BlogCategory> blogCategories = await _appDbContext.BlogCategories
                .Where(m => !m.IsDeleted)
                .Include(m => m.Blogs)
                .ToListAsync();

            IEnumerable<Tag> tags = await _appDbContext.Tags
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            BlogVM model = new()
            {
                Blogs = blogs,
                BlogCategories = blogCategories,
                Tags = tags
            };

            return View(model);
        }

        public async Task<IActionResult> Search(string search)
        {
            BlogVM searchBlog = new();

            if (search != null)
            {
                searchBlog.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.Name.ToLower()
                    .Contains(search.ToLower()) && !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                searchBlog.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", searchBlog);
        }

        public async Task<IActionResult> FilterTag(int id)
        {
            BlogVM model = new();

            if (id != null)
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.BlogTags
                    .Where(m => m.TagId == id)
                    .FirstOrDefault().TagId == id)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", model);
        }

        public async Task<IActionResult> FilterCategory(int id)
        {
            BlogVM model = new();

            if (id != null)
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted && m.BlogCategoryId == id)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }
            else
            {
                model.Blogs = await _appDbContext.Blogs
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.BlogTags)
                    .Include(m => m.BlogCategory)
                    .ToListAsync();
            }

            return PartialView("_BlogsPartial", model);
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
