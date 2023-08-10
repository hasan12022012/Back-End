using Backend.DataAccessLayer;
using Backend.Models;
using Backend.ViewModels.AboutViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AboutController(AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task<int> GetPageCount(int take)
        {
            int aboutCount = await _context.Abouts.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)aboutCount / take);
        }

        private List<AboutListVM> GetMapDatas(List<About> abouts)
        {
            List<AboutListVM> aboutList = new();

            foreach (var about in abouts)
            {
                AboutListVM newAbout = new()
                {
                    Id = about.Id,
                    Title = about.Title,
                    Content = about.Content,
                };

                aboutList.Add(newAbout);
            }

            return aboutList;
        }

        private async Task<About> GetByIdAsync(int id)
        {
            return await _context.Abouts
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
