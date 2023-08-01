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
                .Include(m => m.BlogCategory)
                .ToListAsync();

            BlogVM model = new()
            {
                Blogs = blogs
            };

            return View(model);
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
