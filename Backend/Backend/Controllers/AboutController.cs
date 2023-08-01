using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AboutController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<About> abouts = await _appDbContext.Abouts
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            AboutVM model = new()
            {
                Abouts = abouts
            };

            return View(model);
        }
    }
}
