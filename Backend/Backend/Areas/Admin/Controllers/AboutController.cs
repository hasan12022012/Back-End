using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.AboutViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<About> abouts = await _context.Abouts
                  .Where(m => !m.IsDeleted)
                  .OrderByDescending(m => m.Id)
                  .Skip((page * take) - take)
                  .Take(take)
                  .ToListAsync();

            List<AboutListVM> mapDatas = GetMapDatas(abouts);
            int count = await GetPageCount(take);

            Paginate<AboutListVM> result = new(mapDatas, page, count);

            return View(result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutCreateVM about)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            About newAbout = new()
            {
                Title = about.Title,
                Content = about.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _context.Abouts.AddAsync(newAbout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            About dbAbout = await GetByIdAsync((int)id);

            AboutUpdateVM updatedAbout = new()
            {
                Id = dbAbout.Id,
                Title = dbAbout.Title,
                Content = dbAbout.Content,
            };

            return View(updatedAbout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, AboutUpdateVM updatedAbout)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid) return View(updatedAbout);

            About dbAbout = await GetByIdAsync((int)id);

            dbAbout.Title = updatedAbout.Title;
            dbAbout.Content = updatedAbout.Content;
            dbAbout.UpdatedAt = DateTime.UtcNow;
            dbAbout.UpdatedBy = user.UserName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            About about = await GetByIdAsync((int)id);

            if (about == null) return NotFound();

            AboutDetailVM aboutDetail = new()
            {
                Title = about.Title,
                Content = about.Content
            };

            return View(aboutDetail);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            About about = await _context.Abouts
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();

            if (about == null) return NotFound();

            about.IsDeleted = true;
            about.DeletedAt = DateTime.UtcNow;
            about.DeletedBy = user.UserName;

            await _context.SaveChangesAsync();

            return Ok();
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
