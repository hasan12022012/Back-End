using Backend.DataAccessLayer;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels.BannerViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public BannerController(AppDbContext context,
            IWebHostEnvironment environment,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            Banner banner = await _context.Banners
                .Where(m => !m.IsDeleted)
                .FirstOrDefaultAsync();

            BannerListVM model = new()
            {
                Id = banner.Id,
                Image = banner.Image
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Banner dbBanner = await GetByIdAsync((int)id);

            BannerUpdateVM model = new()
            {
                Id = dbBanner.Id,
                Image = dbBanner.Image
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BannerUpdateVM updatedBanner)
        {
            if (!ModelState.IsValid) return View(updatedBanner);

            Banner dbBanner = await GetByIdAsync(id);

            if (updatedBanner.Photo != null)
            {
                if (!updatedBanner.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image type");
                    return View(updatedBanner);
                }

                if (!updatedBanner.Photo.CheckFileSize(1000))
                {
                    ModelState.AddModelError("Photo", "Please, choose correct image size");
                    return View(updatedBanner);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + updatedBanner.Photo.FileName;

                string path = FileType.GetFilePath(_environment.WebRootPath, "assets/img", fileName);

                await FileType.SaveFile(path, updatedBanner.Photo);

                dbBanner.Image = fileName;
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            dbBanner.UpdatedAt = DateTime.UtcNow;
            dbBanner.UpdatedBy = user.UserName;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Banner> GetByIdAsync(int id)
        {
            return await _context.Banners
                .Where(m => !m.IsDeleted && m.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
